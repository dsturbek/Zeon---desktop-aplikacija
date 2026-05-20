using BussinessLogicLayer;
using EntitiesLayer.Entities;
using ApiServiceLayer.Services;
using ApiServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zeon
{
    public partial class UcCommunication : UserControl
    {
        private ChatService chatService = new ChatService();
        private readonly NotificationApiService _notificationService = new NotificationApiService();
        private int trainerId;
        private int? activeConversationId;
        private List<ConversationDisplayItem> allConversationItems;
        private List<NotificationDisplayItem> allNotifications = new List<NotificationDisplayItem>();
        private NotificationResponse _lastNotificationResponse;

        public UcCommunication(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            InitializeFeedbackTab();
            this.Loaded += UcCommunication_Loaded;
        }

        private void InitializeFeedbackTab()
        {
            feedbackContent.Content = new FeedbackOverview(trainerId);
        }

        private async void UcCommunication_Loaded(object sender, RoutedEventArgs e)
        {
            await Load();
        }

        public async Task Load()
        {
            await ShowConversations();
            await LoadNotifications();
        }

        private async Task LoadNotifications()
        {
            try
            {
                _lastNotificationResponse = await _notificationService.GetNotificationsAsync(trainerId);

                allNotifications.Clear();

                if (_lastNotificationResponse.Notifications != null)
                {
                    foreach (var msg in _lastNotificationResponse.Notifications.Messages ?? new List<MessageNotification>())
                    {
                        allNotifications.Add(new NotificationDisplayItem
                        {
                            Type = NotificationType.Message,
                            Id = msg.IdMessage,
                            Title = $"Nova poruka - {msg.ClientName}",
                            Description = msg.MessageContent?.Length > 50
                                ? msg.MessageContent.Substring(0, 50) + "..."
                                : msg.MessageContent,
                            IsSeen = false
                        });
                    }

                    foreach (var fb in _lastNotificationResponse.Notifications.FeedbackTraining ?? new List<TrainingFeedbackNotification>())
                    {
                        allNotifications.Add(new NotificationDisplayItem
                        {
                            Type = NotificationType.TrainingFeedback,
                            Id = fb.IdFeedback,
                            Title = $"Povratna informacija - {fb.ClientName}",
                            Description = $"{fb.ExerciseName}: {fb.CompletedSets}x{fb.CompletedReps}" +
                                (fb.CompletedWeight.HasValue ? $" @ {fb.CompletedWeight}kg" : ""),
                            IsSeen = false
                        });
                    }

                    foreach (var fb in _lastNotificationResponse.Notifications.FeedbackMeal ?? new List<MealFeedbackNotification>())
                    {
                        allNotifications.Add(new NotificationDisplayItem
                        {
                            Type = NotificationType.MealFeedback,
                            Id = fb.IdFeedbackMeal,
                            Title = $"Komentar prehrane - {fb.ClientName}",
                            Description = fb.Comment?.Length > 50
                                ? fb.Comment.Substring(0, 50) + "..."
                                : fb.Comment,
                            IsSeen = false
                        });
                    }
                }

                UpdateNotificationsUI();
            }
            catch (Exception ex)
            {
                txtNoNotifications.Text = $"Greška pri dohvaćanju obavijesti: {ex.Message}";
                txtNoNotifications.Visibility = Visibility.Visible;
            }
        }

        private void UpdateNotificationsUI()
        {
            if (allNotifications.Count == 0)
            {
                txtNoNotifications.Text = "Nema novih obavijesti.";
                txtNoNotifications.Visibility = Visibility.Visible;
                icNotifications.ItemsSource = null;
            }
            else
            {
                txtNoNotifications.Visibility = Visibility.Collapsed;
                icNotifications.ItemsSource = null;
                icNotifications.ItemsSource = allNotifications;
            }
        }

        private async void btnRefreshNotifications_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshNotifications.IsEnabled = false;
            btnRefreshNotifications.Content = "Učitavanje...";

            await LoadNotifications();

            btnRefreshNotifications.Content = "Osvježi";
            btnRefreshNotifications.IsEnabled = true;
        }

        private void NotificationCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var item = checkBox?.Tag as NotificationDisplayItem;

            if (item != null && item.IsSeen)
            {
                switch (item.Type)
                {
                    case NotificationType.Message:
                        _notificationService.MarkMessageAsSeen(trainerId, item.Id);
                        break;
                    case NotificationType.TrainingFeedback:
                        _notificationService.MarkTrainingFeedbackAsSeen(trainerId, item.Id);
                        break;
                    case NotificationType.MealFeedback:
                        _notificationService.MarkMealFeedbackAsSeen(trainerId, item.Id);
                        break;
                }

                RefreshMainWindowBadge();
            }
        }

        private void RefreshMainWindowBadge()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.RefreshNotificationBadge();
        }

        public async Task ShowConversations()
        {
            try
            {
                var conversations = await Task.Run(() => chatService.GetAllConversations(trainerId));

                allConversationItems = conversations.Select(c => new ConversationDisplayItem
                {
                    ConversationId = c.id_conversation,
                    ClientName = c.Client?.name_surname ?? "Nepoznat",
                    MessageCount = c.Messages.Count + " poruka",
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.id_message)
                        .Select(m => m.message_content)
                        .FirstOrDefault() ?? "Nema poruka"
                }).ToList();

                lstConversations.ItemsSource = allConversationItems;
                txtNoConversations.Visibility = allConversationItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnNewConversation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewConversationDialog(trainerId);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var conversation = await Task.Run(() => chatService.GetOrCreateConversation(trainerId, dialog.SelectedClient.id_client));
                    await ShowConversations();

                    var item = allConversationItems?.FirstOrDefault(c => c.ConversationId == conversation.id_conversation);
                    if (item != null)
                    {
                        lstConversations.SelectedItem = item;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtSearchConversations_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allConversationItems == null) return;
            string search = txtSearchConversations.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(search))
            {
                lstConversations.ItemsSource = allConversationItems;
            }
            else
            {
                lstConversations.ItemsSource = allConversationItems
                    .Where(c => c.ClientName.ToLower().Contains(search))
                    .ToList();
            }
        }

        private async void lstConversations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstConversations.SelectedItem as ConversationDisplayItem;
            if (selected == null) return;

            activeConversationId = selected.ConversationId;
            btnOpenConversation_Click(selected);
            await LoadConversation(selected.ConversationId);
        }

        private void btnOpenConversation_Click(ConversationDisplayItem item)
        {
            txtChatClientName.Text = item.ClientName;
            txtChatStatus.Text = item.MessageCount;

            string initials = GetInitials(item.ClientName);
            txtChatAvatar.Text = initials;
        }

        private async Task LoadConversation(int conversationId)
        {
            try
            {
                var messages = await Task.Run(() => chatService.GetConversationHistory(conversationId));

                var displayMessages = messages.Select(m =>
                {
                    bool isSent = m.SenderTrainerId.HasValue;
                    return new MessageDisplayItem
                    {
                        Content = m.message_content ?? "",
                        SenderName = isSent ? "Ti" : (m.Client?.name_surname ?? "Klijent"),
                        IsSent = isSent,
                        SentVisibility = isSent ? Visibility.Visible : Visibility.Collapsed,
                        ReceivedVisibility = isSent ? Visibility.Collapsed : Visibility.Visible
                    };
                }).ToList();

                wpConversation.ItemsSource = displayMessages;
                txtNoMessages.Visibility = displayMessages.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                scrollMessages.Visibility = displayMessages.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

                scrollMessages.ScrollToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(txtMessage.Text) && txtMessage.Text.Length <= 255;
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await SendCurrentMessage();
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SendCurrentMessage();
                e.Handled = true;
            }
        }

        private async Task SendCurrentMessage()
        {
            if (!activeConversationId.HasValue)
            {
                MessageBox.Show("Odaberite razgovor.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Validate())
            {
                MessageBox.Show("Poruka ne može biti prazna (max 255 znakova).", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var message = new Message
                {
                    message_content = txtMessage.Text.Trim(),
                    SenderTrainerId = trainerId
                };

                await Task.Run(() => chatService.SendMessage(activeConversationId.Value, message));

                txtMessage.Clear();
                await LoadConversation(activeConversationId.Value);
                await ShowConversations();

                // Re-select active conversation
                var item = allConversationItems?.FirstOrDefault(c => c.ConversationId == activeConversationId.Value);
                if (item != null)
                {
                    lstConversations.SelectedItem = item;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Trim().Split(' ');
            if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            return parts[0][0].ToString().ToUpper();
        }
    }

    public class ConversationDisplayItem
    {
        public int ConversationId { get; set; }
        public string ClientName { get; set; }
        public string MessageCount { get; set; }
        public string LastMessage { get; set; }
    }

    public class MessageDisplayItem
    {
        public string Content { get; set; }
        public string SenderName { get; set; }
        public bool IsSent { get; set; }
        public Visibility SentVisibility { get; set; }
        public Visibility ReceivedVisibility { get; set; }
    }

    public enum NotificationType
    {
        Message,
        TrainingFeedback,
        MealFeedback
    }

    public class NotificationDisplayItem
    {
        public NotificationType Type { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsSeen { get; set; }
    }
}

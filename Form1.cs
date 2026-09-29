using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartBuss
{
    public partial class Form1 : Form
    {
        private Panel contentPanel;
        private Label connectionLabel;
        private Label clockLabel;

        private Timer clockTimer;
        private Timer simulationTimer;
        private Timer orderTimer;
        private Timer robotTimer;

        private readonly Random random = new Random();

        private readonly Color Navy = Color.FromArgb(27, 52, 77);
        private readonly Color Blue = Color.FromArgb(47, 111, 191);
        private readonly Color LightBlue = Color.FromArgb(239, 246, 253);
        private readonly Color Green = Color.FromArgb(38, 126, 91);
        private readonly Color Orange = Color.FromArgb(202, 126, 38);
        private readonly Color Red = Color.FromArgb(181, 69, 69);
        private readonly Color Background = Color.FromArgb(245, 247, 250);
        private readonly Color TextDark = Color.FromArgb(40, 52, 62);
        private readonly Color TextMuted = Color.FromArgb(105, 116, 128);
        private readonly Color Border = Color.FromArgb(224, 229, 235);

        private int currentSpeed = 38;
        private int batteryLevel = 68;
        private int solarEfficiency = 74;
        private int temperature = 22;

        private decimal cartTotal;
        private readonly List<CartItem> cartItems =
            new List<CartItem>();

        private OrderService orderService;
        private RobotService robotService;
        private NotificationService notificationService;

        private bool robotIsCleaning;

        private Label robotStatusLabel;
        private Label robotLocationLabel;
        private Label robotLegsLabel;
        private Label robotBatteryLabel;
        private Label robotTimeLabel;
        private Label robotAlertLabel;
        private ProgressBar robotProgressBar;
        private ComboBox robotAreaComboBox;
        private ComboBox robotMethodComboBox;
        private ComboBox robotDurationComboBox;
        private CheckedListBox robotZonesCheckedListBox;
        private ListBox robotDetectedItemsListBox;

        public Form1()
        {
            InitializeComponent();

            orderService = new OrderService();
            notificationService = new NotificationService();

            robotService = new RobotService(batteryLevel);
            robotService.ObjectDetected += RobotService_ObjectDetected;
            robotService.CleaningCompleted += RobotService_CleaningCompleted;

            ConfigureForm();
            BuildApplication();
            StartTimers();
            ShowPassengerDashboard();
        }

        private void ConfigureForm()
        {
            BackColor = Background;
            Font = new Font("Segoe UI", 10F);
            Text = "SmartBuss - Έξυπνο Διώροφο Τουριστικό Λεωφορείο";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void BuildApplication()
        {
            Panel header = BuildHeader();
            Panel sidebar = BuildSidebar();

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                Padding = new Padding(28)
            };

            Controls.Add(contentPanel);
            Controls.Add(sidebar);
            Controls.Add(header);
        }

        private Panel BuildHeader()
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = Navy
            };

            Label brand = new Label
            {
                Text = "SmartBuss",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Location = new Point(25, 13)
            };

            Label subtitle = new Label
            {
                Text = "Έξυπνο διώροφο τουριστικό λεωφορείο",
                AutoSize = true,
                ForeColor = Color.FromArgb(210, 222, 236),
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(28, 47)
            };

            connectionLabel = new Label
            {
                Text = "● Συνδεδεμένο",
                AutoSize = true,
                ForeColor = Color.FromArgb(152, 221, 174),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(970, 20)
            };

            clockLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(1145, 26)
            };

            header.Resize += delegate
            {
                connectionLabel.Left = header.Width - 290;
                clockLabel.Left = header.Width - 105;
            };

            header.Controls.Add(brand);
            header.Controls.Add(subtitle);
            header.Controls.Add(connectionLabel);
            header.Controls.Add(clockLabel);

            return header;
        }

        private Panel BuildSidebar()
        {
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 215,
                BackColor = Color.White,
                Padding = new Padding(14, 20, 14, 15)
            };

            Label menuLabel = new Label
            {
                Text = "ΚΕΝΤΡΙΚΟ ΜΕΝΟΥ",
                AutoSize = true,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(18, 20)
            };

            Button homeButton = CreateMenuButton("Επισκόπηση");
            Button routeButton = CreateMenuButton("Διαδρομή");
            Button ordersButton = CreateMenuButton("Παραγγελίες");
            Button trackingButton = CreateMenuButton("Παρακολούθηση");
            Button driverButton = CreateMenuButton("Οθόνη οδηγού");
            Button employeeButton = CreateMenuButton("Τεχνικός έλεγχος");
            Button robotButton = CreateMenuButton("Ρομπότ καθαρισμού");
            Button notificationsButton = CreateMenuButton("Ειδοποιήσεις");
            Button helpButton = CreateMenuButton("Βοήθεια");

            Button[] buttons =
            {
                homeButton,
                routeButton,
                ordersButton,
                trackingButton,
                driverButton,
                employeeButton,
                robotButton,
                notificationsButton,
                helpButton
            };

            int y = 55;

            foreach (Button button in buttons)
            {
                button.Location = new Point(14, y);
                y += 42;
                sidebar.Controls.Add(button);
            }

            homeButton.Click += delegate { ShowPassengerDashboard(); };
            routeButton.Click += delegate { ShowRoutePage(); };
            ordersButton.Click += delegate { ShowOrderPage(); };
            trackingButton.Click += delegate { ShowOrderTrackingPage(); };
            driverButton.Click += delegate { ShowDriverPage(); };
            employeeButton.Click += delegate { ShowEmployeePage(); };
            robotButton.Click += delegate { ShowRobotPage(); };
            notificationsButton.Click += delegate { ShowNotificationsPage(); };
            helpButton.Click += delegate { ShowHelpPage(); };

            Label busInfo = new Label
            {
                Text =
                    "Λεωφορείο 405\n" +
                    "Γραμμή: Downtown Express\n" +
                    "Διώροφο όχημα\n" +
                    "Τρέχουσα στάση: Ακρόπολη",
                AutoSize = true,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(18, 465)
            };

            sidebar.Controls.Add(menuLabel);
            sidebar.Controls.Add(busInfo);

            return sidebar;
        }

        private Button CreateMenuButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Width = 180,
                Height = 36,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextDark,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;

            button.MouseEnter += delegate
            {
                button.BackColor = LightBlue;
                button.ForeColor = Blue;
            };

            button.MouseLeave += delegate
            {
                button.BackColor = Color.White;
                button.ForeColor = TextDark;
            };

            return button;
        }

        private void PreparePage(string title, string subtitle)
        {
            contentPanel.Controls.Clear();

            Label pageTitle = new Label
            {
                Text = title,
                AutoSize = true,
                ForeColor = TextDark,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                Location = new Point(28, 20)
            };

            Label pageSubtitle = new Label
            {
                Text = subtitle,
                AutoSize = true,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(31, 58)
            };

            contentPanel.Controls.Add(pageTitle);
            contentPanel.Controls.Add(pageSubtitle);
        }

        private void ShowPassengerDashboard()
        {
            PreparePage(
                "Επισκόπηση διαδρομής",
                "Η τρέχουσα κατάσταση του διώροφου λεωφορείου.");

            Panel statusCard = CreateCard(0, 100, 980, 115);
            AddCardTitle(statusCard, "Τρέχουσα διαδρομή");

            Label route = CreateTextLabel(
                "Downtown Express  •  Γραμμή 405",
                12,
                TextDark);
            route.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            route.Location = new Point(22, 48);
            route.AutoSize = true;

            Label nextStop = CreateTextLabel(
                "Επόμενη στάση: Ακρόπολη",
                10,
                Blue);
            nextStop.Location = new Point(23, 80);
            nextStop.AutoSize = true;

            Label arrival = CreateTextLabel(
                "Άφιξη σε περίπου 8 λεπτά",
                10,
                TextMuted);
            arrival.Location = new Point(260, 80);
            arrival.AutoSize = true;

            Label weather = CreateTextLabel(
                "Ηλιοφάνεια  •  26°C",
                10,
                TextMuted);
            weather.Location = new Point(770, 65);
            weather.AutoSize = true;

            statusCard.Controls.Add(route);
            statusCard.Controls.Add(nextStop);
            statusCard.Controls.Add(arrival);
            statusCard.Controls.Add(weather);

            Panel routeCard = CreateCard(0, 235, 475, 260);
            AddCardTitle(routeCard, "Πληροφορίες διαδρομής");

            Label routeMap = CreateTextLabel(
                "Κάτω όροφος: Σύνταγμα → Ακρόπολη → Μουσείο\n\n" +
                "Πάνω όροφος: Εθνικός Κήπος → Παραλία",
                11,
                Blue);
            routeMap.TextAlign = ContentAlignment.MiddleCenter;
            routeMap.BackColor = LightBlue;
            routeMap.Location = new Point(20, 58);
            routeMap.Size = new Size(430, 115);

            Label routeInfo = CreateTextLabel(
                "Το λεωφορείο διαθέτει κάτω και πάνω όροφο. " +
                "Μπορείτε να αποβιβαστείτε σε οποιαδήποτε στάση.",
                9.5F,
                TextMuted);
            routeInfo.Location = new Point(22, 195);
            routeInfo.MaximumSize = new Size(420, 40);
            routeInfo.AutoSize = true;

            routeCard.Controls.Add(routeMap);
            routeCard.Controls.Add(routeInfo);

            Panel servicesCard = CreateCard(505, 235, 475, 260);
            AddCardTitle(servicesCard, "Διαθέσιμες υπηρεσίες");

            AddActionButton(
                servicesCard,
                "Αξιοθέατα και ξενάγηση",
                "Πληροφορίες για τα αξιοθέατα της επόμενης στάσης.",
                20,
                58,
                delegate { ShowRoutePage(); });

            AddActionButton(
                servicesCard,
                "Παραγγελία από καφετέρια",
                "Καφέδες, ροφήματα και μικρά γεύματα.",
                20,
                118,
                delegate { ShowOrderPage(); });

            AddActionButton(
                servicesCard,
                "Τουριστική πλοήγηση",
                "Οδηγίες προς αξιοθέατα και στάσεις.",
                20,
                178,
                delegate { ShowNavigationPage(); });

            Panel infoCard = CreateCard(0, 515, 980, 105);
            AddCardTitle(infoCard, "Κατάσταση συστήματος");

            Label systemInfo = CreateTextLabel(
                "Η σύνδεση με το λεωφορείο είναι ενεργή. " +
                "Οι δύο όροφοι και τα βασικά συστήματα λειτουργούν κανονικά.",
                10,
                Green);
            systemInfo.Location = new Point(22, 57);
            systemInfo.AutoSize = true;

            infoCard.Controls.Add(systemInfo);

            contentPanel.Controls.Add(statusCard);
            contentPanel.Controls.Add(routeCard);
            contentPanel.Controls.Add(servicesCard);
            contentPanel.Controls.Add(infoCard);
        }

        private void ShowRoutePage()
        {
            PreparePage(
                "Διαδρομή και αξιοθέατα",
                "Πληροφορίες για τα σημεία ενδιαφέροντος κοντά στις στάσεις.");

            Panel routeCard = CreateCard(0, 100, 980, 140);
            AddCardTitle(routeCard, "Η διαδρομή του διώροφου λεωφορείου");

            Label route = CreateTextLabel(
                "Σύνταγμα → Ακρόπολη → Μουσείο → Εθνικός Κήπος → Παραλία",
                12,
                Blue);
            route.Location = new Point(24, 62);
            route.AutoSize = true;

            Label current = CreateTextLabel(
                "Τρέχουσα θέση: πριν από τη στάση Ακρόπολη",
                9.5F,
                TextMuted);
            current.Location = new Point(25, 98);
            current.AutoSize = true;

            routeCard.Controls.Add(route);
            routeCard.Controls.Add(current);
            contentPanel.Controls.Add(routeCard);

            AddAttraction(
                "Ακρόπολη",
                "Ιστορικός χώρος με πανοραμική θέα στην πόλη.",
                "12 λεπτά από τη στάση",
                0,
                265);

            AddAttraction(
                "Μουσείο Ακρόπολης",
                "Αρχαιολογικά ευρήματα και διαδραστικές εκθέσεις.",
                "18 λεπτά από τη στάση",
                330,
                265);

            AddAttraction(
                "Εθνικός Κήπος",
                "Ήσυχη διαδρομή με πράσινο στο κέντρο της πόλης.",
                "25 λεπτά από τη στάση",
                660,
                265);
        }

        private void AddAttraction(
            string title,
            string description,
            string distance,
            int x,
            int y)
        {
            Panel card = CreateCard(x, y, 300, 220);
            AddCardTitle(card, title);

            Label descriptionLabel = CreateTextLabel(
                description,
                9.5F,
                TextMuted);
            descriptionLabel.Location = new Point(20, 58);
            descriptionLabel.MaximumSize = new Size(260, 45);
            descriptionLabel.AutoSize = true;

            Label distanceLabel = CreateTextLabel(
                distance,
                9.5F,
                Blue);
            distanceLabel.Location = new Point(20, 125);
            distanceLabel.AutoSize = true;

            Button audioButton = CreatePrimaryButton("Ηχητική ξενάγηση");
            audioButton.Location = new Point(20, 160);
            audioButton.Size = new Size(155, 34);

            audioButton.Click += delegate
            {
                MessageBox.Show(
                    "Ξεκινά η ηχητική ξενάγηση για:\n" + title,
                    "Ηχητική ξενάγηση",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            card.Controls.Add(descriptionLabel);
            card.Controls.Add(distanceLabel);
            card.Controls.Add(audioButton);
            contentPanel.Controls.Add(card);
        }

        private void ShowOrderPage()
        {
            PreparePage(
                "Παραγγελία από συνεργαζόμενες καφετέριες",
                "Επιλέξτε προϊόντα και την επόμενη στάση παράδοσης.");

            Panel storesCard = CreateCard(0, 100, 620, 540);
            AddCardTitle(storesCard, "Μενού καταστημάτων");

            AddProductButton(storesCard, "Espresso", "Coffee & Café", 2.50m, 25, 65);
            AddProductButton(storesCard, "Cappuccino", "Coffee & Café", 3.20m, 25, 125);
            AddProductButton(storesCard, "Κρουασάν", "Coffee & Café", 2.80m, 25, 185);
            AddProductButton(storesCard, "Burger", "Fast Food Central", 6.50m, 315, 65);
            AddProductButton(storesCard, "Pizza slice", "Fast Food Central", 4.50m, 315, 125);
            AddProductButton(storesCard, "Αναψυκτικό", "Fast Food Central", 2.20m, 315, 185);
            AddProductButton(storesCard, "Chicken wrap", "Healthy Hub", 6.80m, 25, 275);
            AddProductButton(storesCard, "Smoothie", "Healthy Hub", 4.20m, 315, 275);

            Label note = CreateTextLabel(
                "Η πληρωμή γίνεται με εικονική κάρτα. " +
                "Η κατάσταση της παραγγελίας ενημερώνεται αυτόματα.",
                9,
                TextMuted);
            note.Location = new Point(25, 475);
            note.MaximumSize = new Size(560, 35);
            note.AutoSize = true;
            storesCard.Controls.Add(note);

            Panel cartCard = CreateCard(645, 100, 335, 540);
            AddCardTitle(cartCard, "Το καλάθι σας");

            FlowLayoutPanel itemsPanel = new FlowLayoutPanel
            {
                Name = "itemsPanel",
                Location = new Point(20, 58),
                Size = new Size(290, 280),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            Label totalLabel = new Label
            {
                Name = "totalLabel",
                Text = "Σύνολο: 0,00 €",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(20, 360)
            };

            Button checkoutButton = CreatePrimaryButton("Συνέχεια στην πληρωμή");
            checkoutButton.Location = new Point(20, 410);
            checkoutButton.Size = new Size(290, 42);
            checkoutButton.Click += delegate
            {
                CheckoutOrder(itemsPanel, totalLabel);
            };

            Button clearButton = CreateSecondaryButton("Καθαρισμός καλαθιού");
            clearButton.Location = new Point(20, 465);
            clearButton.Size = new Size(290, 35);
            clearButton.Click += delegate
            {
                cartItems.Clear();
                cartTotal = 0;
                RefreshCart(itemsPanel, totalLabel);
            };

            cartCard.Controls.Add(itemsPanel);
            cartCard.Controls.Add(totalLabel);
            cartCard.Controls.Add(checkoutButton);
            cartCard.Controls.Add(clearButton);

            contentPanel.Controls.Add(storesCard);
            contentPanel.Controls.Add(cartCard);

            RefreshCart(itemsPanel, totalLabel);
        }

        private void AddProductButton(
            Panel parent,
            string product,
            string store,
            decimal price,
            int x,
            int y)
        {
            Panel productPanel = new Panel
            {
                Width = 265,
                Height = 48,
                Location = new Point(x, y),
                BackColor = Color.FromArgb(250, 251, 253),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label productLabel = CreateTextLabel(
                product + "\n" + store,
                9,
                TextDark);
            productLabel.Location = new Point(10, 6);
            productLabel.AutoSize = true;

            Label priceLabel = CreateTextLabel(
                price.ToString("0.00") + " €",
                9.5F,
                Blue);
            priceLabel.Location = new Point(165, 14);
            priceLabel.AutoSize = true;

            Button addButton = CreatePrimaryButton("+");
            addButton.Location = new Point(220, 8);
            addButton.Size = new Size(32, 30);

            addButton.Click += delegate
            {
                CartItem existing = cartItems.FirstOrDefault(
                    item => item.Name == product);

                if (existing == null)
                {
                    cartItems.Add(new CartItem
                    {
                        Name = product,
                        Store = store,
                        Price = price,
                        Quantity = 1
                    });
                }
                else
                {
                    existing.Quantity++;
                }

                cartTotal += price;
                RefreshVisibleCart();
            };

            productPanel.Controls.Add(productLabel);
            productPanel.Controls.Add(priceLabel);
            productPanel.Controls.Add(addButton);
            parent.Controls.Add(productPanel);
        }

        private void RefreshVisibleCart()
        {
            Panel cartCard = contentPanel.Controls
                .OfType<Panel>()
                .FirstOrDefault(panel => panel.Width == 335);

            if (cartCard == null)
            {
                return;
            }

            FlowLayoutPanel itemsPanel = cartCard.Controls
                .OfType<FlowLayoutPanel>()
                .FirstOrDefault();

            Label totalLabel = cartCard.Controls
                .OfType<Label>()
                .FirstOrDefault(label => label.Name == "totalLabel");

            if (itemsPanel != null && totalLabel != null)
            {
                RefreshCart(itemsPanel, totalLabel);
            }
        }

        private void RefreshCart(
            FlowLayoutPanel itemsPanel,
            Label totalLabel)
        {
            itemsPanel.Controls.Clear();

            if (cartItems.Count == 0)
            {
                Label empty = CreateTextLabel(
                    "Το καλάθι είναι άδειο.\n\nΠροσθέστε προϊόντα από το μενού.",
                    9.5F,
                    TextMuted);
                empty.AutoSize = true;
                empty.Margin = new Padding(8, 20, 0, 0);
                itemsPanel.Controls.Add(empty);
            }
            else
            {
                foreach (CartItem item in cartItems)
                {
                    Panel itemPanel = new Panel
                    {
                        Width = 265,
                        Height = 57,
                        BackColor = LightBlue,
                        Margin = new Padding(3)
                    };

                    Label itemLabel = CreateTextLabel(
                        item.Name + "  x" + item.Quantity + "\n" +
                        item.Store + "  •  " +
                        item.Subtotal.ToString("0.00") + " €",
                        8.5F,
                        TextDark);
                    itemLabel.Location = new Point(8, 7);
                    itemLabel.AutoSize = true;

                    Button removeButton = CreateSecondaryButton("−");
                    removeButton.Location = new Point(225, 14);
                    removeButton.Size = new Size(30, 28);

                    CartItem selectedItem = item;

                    removeButton.Click += delegate
                    {
                        cartTotal -= selectedItem.Price;

                        if (selectedItem.Quantity > 1)
                        {
                            selectedItem.Quantity--;
                        }
                        else
                        {
                            cartItems.Remove(selectedItem);
                        }

                        RefreshCart(itemsPanel, totalLabel);
                    };

                    itemPanel.Controls.Add(itemLabel);
                    itemPanel.Controls.Add(removeButton);
                    itemsPanel.Controls.Add(itemPanel);
                }
            }

            totalLabel.Text = "Σύνολο: " +
                              cartTotal.ToString("0.00") +
                              " €";
        }

        private void CheckoutOrder(
            FlowLayoutPanel itemsPanel,
            Label totalLabel)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show(
                    "Δεν υπάρχουν προϊόντα στο καλάθι.",
                    "Παραγγελία",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string deliveryStop = SelectDeliveryStop();

            if (string.IsNullOrEmpty(deliveryStop))
            {
                return;
            }

            if (!ShowPaymentDialog())
            {
                return;
            }

            Order order = orderService.CreateOrder(
                new List<CartItem>(cartItems),
                deliveryStop);

            notificationService.Add(
                "Παραγγελία #" + order.Number,
                "Η παραγγελία βρίσκεται σε επεξεργασία από τη " +
                order.Store + ".",
                NotificationPriority.Normal);

            MessageBox.Show(
                "Η παραγγελία #" + order.Number +
                " καταχωρήθηκε επιτυχώς.\n\n" +
                "Κατάστημα: " + order.Store + "\n" +
                "Παράδοση: " + order.DeliveryStop + "\n" +
                "Σύνολο: " + order.Total.ToString("0.00") + " €",
                "Επιτυχής παραγγελία",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            cartItems.Clear();
            cartTotal = 0;
            RefreshCart(itemsPanel, totalLabel);
        }

        private bool ShowPaymentDialog()
        {
            using (Form paymentForm = new Form())
            {
                paymentForm.Text = "Πληρωμή με κάρτα";
                paymentForm.Size = new Size(430, 360);
                paymentForm.StartPosition = FormStartPosition.CenterParent;
                paymentForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                paymentForm.MaximizeBox = false;
                paymentForm.MinimizeBox = false;

                Label title = new Label
                {
                    Text = "Στοιχεία εικονικής πληρωμής",
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(28, 22)
                };

                Label amount = new Label
                {
                    Text = "Ποσό πληρωμής: " +
                           cartTotal.ToString("0.00") + " €",
                    AutoSize = true,
                    Location = new Point(30, 62)
                };

                Label cardLabel = new Label
                {
                    Text = "Αριθμός κάρτας",
                    AutoSize = true,
                    Location = new Point(30, 100)
                };

                TextBox cardBox = new TextBox
                {
                    Location = new Point(30, 122),
                    Width = 330,
                    MaxLength = 19
                };

                Label expiryLabel = new Label
                {
                    Text = "Λήξη",
                    AutoSize = true,
                    Location = new Point(30, 165)
                };

                TextBox expiryBox = new TextBox
                {
                    Location = new Point(30, 187),
                    Width = 110,
                    MaxLength = 5
                };

                Label cvvLabel = new Label
                {
                    Text = "CVV",
                    AutoSize = true,
                    Location = new Point(170, 165)
                };

                TextBox cvvBox = new TextBox
                {
                    Location = new Point(170, 187),
                    Width = 80,
                    MaxLength = 3,
                    PasswordChar = '*'
                };

                Button payButton = CreatePrimaryButton("Πληρωμή");
                payButton.Location = new Point(30, 250);
                payButton.Size = new Size(145, 40);

                Button cancelButton = CreateSecondaryButton("Άκυρο");
                cancelButton.Location = new Point(205, 250);
                cancelButton.Size = new Size(125, 40);

                payButton.Click += delegate
                {
                    if (cardBox.Text.Trim().Length < 8 ||
                        expiryBox.Text.Trim().Length < 3 ||
                        cvvBox.Text.Trim().Length < 3)
                    {
                        MessageBox.Show(
                            "Συμπληρώστε τα στοιχεία της κάρτας.",
                            "Μη έγκυρα στοιχεία",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    paymentForm.DialogResult = DialogResult.OK;
                };

                cancelButton.Click += delegate
                {
                    paymentForm.DialogResult = DialogResult.Cancel;
                };

                paymentForm.Controls.Add(title);
                paymentForm.Controls.Add(amount);
                paymentForm.Controls.Add(cardLabel);
                paymentForm.Controls.Add(cardBox);
                paymentForm.Controls.Add(expiryLabel);
                paymentForm.Controls.Add(expiryBox);
                paymentForm.Controls.Add(cvvLabel);
                paymentForm.Controls.Add(cvvBox);
                paymentForm.Controls.Add(payButton);
                paymentForm.Controls.Add(cancelButton);

                return paymentForm.ShowDialog(this) ==
                       DialogResult.OK;
            }
        }

        private string SelectDeliveryStop()
        {
            using (Form stopForm = new Form())
            {
                stopForm.Text = "Στάση παράδοσης";
                stopForm.Size = new Size(420, 260);
                stopForm.StartPosition = FormStartPosition.CenterParent;
                stopForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                stopForm.MaximizeBox = false;
                stopForm.MinimizeBox = false;

                Label description = new Label
                {
                    Text = "Επιλέξτε την επόμενη διαθέσιμη στάση.",
                    AutoSize = false,
                    Width = 350,
                    Height = 42,
                    Location = new Point(25, 22)
                };

                ComboBox stops = new ComboBox
                {
                    Location = new Point(25, 82),
                    Width = 350,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                stops.Items.Add("Ακρόπολη - επόμενη στάση");
                stops.Items.Add("Μουσείο - μεθεπόμενη στάση");
                stops.Items.Add("Εθνικός Κήπος - επόμενη διαθέσιμη στάση");
                stops.SelectedIndex = 0;

                Button okButton = CreatePrimaryButton("Επιβεβαίωση");
                okButton.Location = new Point(25, 145);
                okButton.Size = new Size(150, 38);

                Button cancelButton = CreateSecondaryButton("Άκυρο");
                cancelButton.Location = new Point(210, 145);
                cancelButton.Size = new Size(150, 38);

                okButton.Click += delegate
                {
                    stopForm.DialogResult = DialogResult.OK;
                };

                cancelButton.Click += delegate
                {
                    stopForm.DialogResult = DialogResult.Cancel;
                };

                stopForm.Controls.Add(description);
                stopForm.Controls.Add(stops);
                stopForm.Controls.Add(okButton);
                stopForm.Controls.Add(cancelButton);

                if (stopForm.ShowDialog(this) == DialogResult.OK)
                {
                    return stops.SelectedItem.ToString();
                }

                return string.Empty;
            }
        }

        private void ShowOrderTrackingPage()
        {
            PreparePage(
                "Παρακολούθηση παραγγελιών",
                "Η κατάσταση ενημερώνεται αυτόματα κάθε 10 δευτερόλεπτα.");

            Panel ordersCard = CreateCard(0, 100, 980, 520);
            AddCardTitle(ordersCard, "Οι παραγγελίες σας");

            IReadOnlyList<Order> orders = orderService.GetAll();

            if (orders.Count == 0)
            {
                Label empty = CreateTextLabel(
                    "Δεν υπάρχουν παραγγελίες.",
                    10,
                    TextMuted);
                empty.Location = new Point(25, 70);
                empty.AutoSize = true;
                ordersCard.Controls.Add(empty);
            }
            else
            {
                int y = 65;

                foreach (Order order in orders)
                {
                    Panel row = new Panel
                    {
                        Width = 900,
                        Height = 82,
                        Location = new Point(22, y),
                        BackColor = LightBlue,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Label orderLabel = CreateTextLabel(
                        "#" + order.Number + "  " + order.Store,
                        10.5F,
                        TextDark);
                    orderLabel.Font = new Font(
                        "Segoe UI",
                        10.5F,
                        FontStyle.Bold);
                    orderLabel.Location = new Point(12, 10);
                    orderLabel.AutoSize = true;

                    Label details = CreateTextLabel(
                        "Παράδοση: " + order.DeliveryStop +
                        "  •  Σύνολο: " +
                        order.Total.ToString("0.00") + " €",
                        9,
                        TextMuted);
                    details.Location = new Point(12, 40);
                    details.AutoSize = true;

                    Label status = CreateTextLabel(
                        order.Status,
                        9.5F,
                        GetStatusColor(order.Status));
                    status.Location = new Point(700, 28);
                    status.AutoSize = true;

                    row.Controls.Add(orderLabel);
                    row.Controls.Add(details);
                    row.Controls.Add(status);
                    ordersCard.Controls.Add(row);

                    y += 95;

                    if (y > 475)
                    {
                        break;
                    }
                }
            }

            contentPanel.Controls.Add(ordersCard);
        }

        private void ShowDriverPage()
        {
            PreparePage(
                "Πίνακας οδηγού",
                "Παρακολούθηση πορείας, ασφάλειας και αποβίβασης επιβατών.");

            Panel speedCard = CreateCard(0, 100, 310, 190);
            AddCardTitle(speedCard, "Πορεία");

            Label speed = CreateMetricLabel(
                currentSpeed + " km/h",
                currentSpeed > 50 ? Red : Green);
            speed.Location = new Point(22, 60);

            Label speedInfo = CreateTextLabel(
                "Όριο ταχύτητας: 50 km/h",
                9.5F,
                TextMuted);
            speedInfo.Location = new Point(25, 130);
            speedInfo.AutoSize = true;

            speedCard.Controls.Add(speed);
            speedCard.Controls.Add(speedInfo);

            Panel safetyCard = CreateCard(330, 100, 310, 190);
            AddCardTitle(safetyCard, "Ασφάλεια θυρών");

            Label safety = CreateTextLabel(
                "Πόρτες: Κλειστές",
                11,
                Green);
            safety.Location = new Point(22, 65);
            safety.AutoSize = true;

            Button doorButton = CreatePrimaryButton("Έλεγχος θυρών");
            doorButton.Location = new Point(22, 120);
            doorButton.Size = new Size(150, 38);

            doorButton.Click += delegate
            {
                safety.Text = "Πόρτες: Ασφαλείς";

                MessageBox.Show(
                    "Ο έλεγχος ολοκληρώθηκε χωρίς προβλήματα.",
                    "Έλεγχος θυρών",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            safetyCard.Controls.Add(safety);
            safetyCard.Controls.Add(doorButton);

            Panel climateCard = CreateCard(660, 100, 320, 190);
            AddCardTitle(climateCard, "Θερμοκρασία");

            Label temp = CreateTextLabel(
                "Θερμοκρασία: " + temperature + "°C",
                11,
                TextDark);
            temp.Location = new Point(22, 62);
            temp.AutoSize = true;

            TrackBar tempBar = new TrackBar
            {
                Minimum = 16,
                Maximum = 30,
                Value = temperature,
                TickFrequency = 2,
                Location = new Point(18, 92),
                Width = 270
            };

            tempBar.Scroll += delegate
            {
                temperature = tempBar.Value;
                temp.Text = "Θερμοκρασία: " +
                            temperature + "°C";
            };

            climateCard.Controls.Add(temp);
            climateCard.Controls.Add(tempBar);

            Panel alertsCard = CreateCard(0, 315, 980, 205);
            AddCardTitle(alertsCard, "Ειδοποιήσεις οδήγησης");

            Label alerts = CreateTextLabel(
                currentSpeed > 50
                    ? "ΠΡΟΕΙΔΟΠΟΙΗΣΗ: Υπέρβαση ορίου ταχύτητας."
                    : "Δεν υπάρχουν ενεργές προειδοποιήσεις.",
                10,
                currentSpeed > 50 ? Red : Green);
            alerts.Location = new Point(22, 62);
            alerts.AutoSize = true;

            CheckBox laneCheck = new CheckBox
            {
                Text = "Παρακολούθηση λωρίδας",
                Checked = true,
                AutoSize = true,
                Location = new Point(22, 100)
            };

            CheckBox fatigueCheck = new CheckBox
            {
                Text = "Έλεγχος κόπωσης οδηγού",
                Checked = true,
                AutoSize = true,
                Location = new Point(230, 100)
            };

            CheckBox passengerCheck = new CheckBox
            {
                Text = "Παρακολούθηση αποβίβασης",
                Checked = true,
                AutoSize = true,
                Location = new Point(22, 140)
            };

            Button exitButton = CreateSecondaryButton(
                "Έλεγχος αποβίβασης");
            exitButton.Location = new Point(310, 135);
            exitButton.Size = new Size(180, 35);

            exitButton.Click += delegate
            {
                MessageBox.Show(
                    "Υπάρχουν επιβάτες που αποβιβάζονται. " +
                    "Οι πόρτες παραμένουν ανοικτές.",
                    "Έλεγχος αποβίβασης",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            };

            alertsCard.Controls.Add(alerts);
            alertsCard.Controls.Add(laneCheck);
            alertsCard.Controls.Add(fatigueCheck);
            alertsCard.Controls.Add(passengerCheck);
            alertsCard.Controls.Add(exitButton);

            contentPanel.Controls.Add(speedCard);
            contentPanel.Controls.Add(safetyCard);
            contentPanel.Controls.Add(climateCard);
            contentPanel.Controls.Add(alertsCard);
        }

        private void ShowEmployeePage()
        {
            PreparePage(
                "Τεχνικός έλεγχος",
                "Διαχείριση ενέργειας, οροφής και τεχνικών συστημάτων.");

            Panel energyCard = CreateCard(0, 100, 310, 220);
            AddCardTitle(energyCard, "Ενέργεια");

            Label solar = CreateMetricLabel(
                solarEfficiency + "%",
                Orange);
            solar.Location = new Point(22, 60);

            Label solarInfo = CreateTextLabel(
                "Απόδοση φωτοβολταϊκών",
                9.5F,
                TextMuted);
            solarInfo.Location = new Point(25, 125);
            solarInfo.AutoSize = true;

            Label battery = CreateTextLabel(
                "Μπαταρία: " + batteryLevel + "%",
                10,
                Green);
            battery.Location = new Point(25, 160);
            battery.AutoSize = true;

            energyCard.Controls.Add(solar);
            energyCard.Controls.Add(solarInfo);
            energyCard.Controls.Add(battery);

            Panel roofCard = CreateCard(330, 100, 310, 270);
            AddCardTitle(roofCard, "Ρυθμιζόμενη οροφή");

            Label roofStatus = CreateTextLabel(
                "Κατάσταση: Ανοιχτή",
                11,
                Green);
            roofStatus.Location = new Point(22, 65);
            roofStatus.AutoSize = true;

            Button openButton = CreatePrimaryButton("Άνοιγμα");
            openButton.Location = new Point(22, 115);
            openButton.Size = new Size(110, 38);

            Button closeButton = CreateSecondaryButton("Κλείσιμο");
            closeButton.Location = new Point(145, 115);
            closeButton.Size = new Size(110, 38);

            Button automaticButton = CreatePrimaryButton(
                "Αυτόματη ρύθμιση");
            automaticButton.Location = new Point(22, 175);
            automaticButton.Size = new Size(180, 38);

            openButton.Click += delegate
            {
                roofStatus.Text = "Κατάσταση: Ανοιχτή";
                roofStatus.ForeColor = Green;
            };

            closeButton.Click += delegate
            {
                roofStatus.Text = "Κατάσταση: Κλειστή";
                roofStatus.ForeColor = TextMuted;
            };

            automaticButton.Click += delegate
            {
                roofStatus.Text = "Κατάσταση: Ανοιχτή";
                roofStatus.ForeColor = Green;

                MessageBox.Show(
                    "Ο καιρός είναι καλός. " +
                    "Η οροφή παραμένει ανοιχτή.",
                    "Αυτόματη ρύθμιση οροφής",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            roofCard.Controls.Add(roofStatus);
            roofCard.Controls.Add(openButton);
            roofCard.Controls.Add(closeButton);
            roofCard.Controls.Add(automaticButton);

            Panel systemsCard = CreateCard(660, 100, 320, 270);
            AddCardTitle(systemsCard, "Συστήματα");

            Label systemStatus = CreateTextLabel(
                "Κλιματισμός: Ενεργός\n" +
                "Φωτισμός: Ενεργός\n" +
                "Αισθητήρες: Ενεργοί\n" +
                "Αριθμός ορόφων: 2",
                10,
                Green);
            systemStatus.Location = new Point(22, 65);
            systemStatus.AutoSize = true;

            systemsCard.Controls.Add(systemStatus);

            Panel reportCard = CreateCard(0, 400, 980, 145);
            AddCardTitle(reportCard, "Τελευταίος τεχνικός έλεγχος");

            Label report = CreateTextLabel(
                "Τα βασικά συστήματα λειτουργούν κανονικά.\n" +
                "Η οροφή ρυθμίζεται σύμφωνα με τις καιρικές συνθήκες.",
                10,
                TextMuted);
            report.Location = new Point(22, 62);
            report.AutoSize = true;

            reportCard.Controls.Add(report);

            contentPanel.Controls.Add(energyCard);
            contentPanel.Controls.Add(roofCard);
            contentPanel.Controls.Add(systemsCard);
            contentPanel.Controls.Add(reportCard);
        }

        private void ShowRobotPage()
        {
            PreparePage(
                "Ρομπότ καθαρισμού",
                "Επιλέξτε περιοχή, σημεία, μέθοδο και διάρκεια καθαρισμού.");

            Panel optionsCard = CreateCard(0, 100, 420, 500);
            AddCardTitle(optionsCard, "Ρυθμίσεις αποστολής");

            Label areaLabel = CreateTextLabel(
                "Κύρια περιοχή",
                9.5F,
                TextDark);
            areaLabel.Location = new Point(22, 62);
            areaLabel.AutoSize = true;

            robotAreaComboBox = new ComboBox
            {
                Location = new Point(22, 85),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            robotAreaComboBox.Items.Add("Όλο το λεωφορείο");
            robotAreaComboBox.Items.Add("Κάτω όροφος");
            robotAreaComboBox.Items.Add("Πάνω όροφος");
            robotAreaComboBox.Items.Add("Περιοχή οδηγού");
            robotAreaComboBox.Items.Add("Σκάλα μεταξύ ορόφων");
            robotAreaComboBox.SelectedIndex = 0;

            Label methodLabel = CreateTextLabel(
                "Μέθοδος καθαρισμού",
                9.5F,
                TextDark);
            methodLabel.Location = new Point(22, 130);
            methodLabel.AutoSize = true;

            robotMethodComboBox = new ComboBox
            {
                Location = new Point(22, 153),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            robotMethodComboBox.Items.Add("Κανονικός καθαρισμός");
            robotMethodComboBox.Items.Add("Εντατικός καθαρισμός");
            robotMethodComboBox.Items.Add("Απολύμανση");
            robotMethodComboBox.Items.Add("Μετά το τέλος διαδρομής");
            robotMethodComboBox.SelectedIndex = 0;

            Label durationLabel = CreateTextLabel(
                "Χρόνος ολοκλήρωσης",
                9.5F,
                TextDark);
            durationLabel.Location = new Point(22, 198);
            durationLabel.AutoSize = true;

            robotDurationComboBox = new ComboBox
            {
                Location = new Point(22, 221),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            robotDurationComboBox.Items.Add("Γρήγορος - 5 λεπτά");
            robotDurationComboBox.Items.Add("Κανονικός - 10 λεπτά");
            robotDurationComboBox.Items.Add("Εντατικός - 20 λεπτά");
            robotDurationComboBox.SelectedIndex = 1;

            Label zonesLabel = CreateTextLabel(
                "Επιμέρους σημεία καθαρισμού",
                9.5F,
                TextDark);
            zonesLabel.Location = new Point(22, 265);
            zonesLabel.AutoSize = true;

            robotZonesCheckedListBox = new CheckedListBox
            {
                Location = new Point(22, 288),
                Width = 330,
                Height = 115,
                CheckOnClick = true
            };

            robotZonesCheckedListBox.Items.Add("Καθίσματα κάτω ορόφου");
            robotZonesCheckedListBox.Items.Add("Καθίσματα πάνω ορόφου");
            robotZonesCheckedListBox.Items.Add("Διάδρομοι");
            robotZonesCheckedListBox.Items.Add("Σκάλα");
            robotZonesCheckedListBox.Items.Add("Περιοχή οδηγού");

            Button startButton = CreatePrimaryButton("Έναρξη αποστολής");
            startButton.Location = new Point(22, 425);
            startButton.Size = new Size(160, 40);
            startButton.Click += delegate { StartRobotCleaning(); };

            Button stopButton = CreateSecondaryButton("Παύση");
            stopButton.Location = new Point(195, 425);
            stopButton.Size = new Size(105, 40);
            stopButton.Click += delegate { StopRobotCleaning(); };

            optionsCard.Controls.Add(areaLabel);
            optionsCard.Controls.Add(robotAreaComboBox);
            optionsCard.Controls.Add(methodLabel);
            optionsCard.Controls.Add(robotMethodComboBox);
            optionsCard.Controls.Add(durationLabel);
            optionsCard.Controls.Add(robotDurationComboBox);
            optionsCard.Controls.Add(zonesLabel);
            optionsCard.Controls.Add(robotZonesCheckedListBox);
            optionsCard.Controls.Add(startButton);
            optionsCard.Controls.Add(stopButton);

            Panel statusCard = CreateCard(445, 100, 535, 500);
            AddCardTitle(statusCard, "Κατάσταση ρομπότ");

            robotStatusLabel = CreateTextLabel(
                "Κατάσταση: Σε αναμονή",
                11,
                TextDark);
            robotStatusLabel.Location = new Point(22, 65);
            robotStatusLabel.AutoSize = true;

            robotLocationLabel = CreateTextLabel(
                "Θέση: Σταθμός φόρτισης",
                9.5F,
                TextMuted);
            robotLocationLabel.Location = new Point(22, 98);
            robotLocationLabel.AutoSize = true;

            robotLegsLabel = CreateTextLabel(
                "Πόδια: Μαζεμένα",
                9.5F,
                TextMuted);
            robotLegsLabel.Location = new Point(22, 127);
            robotLegsLabel.AutoSize = true;

            robotBatteryLabel = CreateTextLabel(
                "Μπαταρία: " + batteryLevel + "%",
                9.5F,
                Green);
            robotBatteryLabel.Location = new Point(22, 156);
            robotBatteryLabel.AutoSize = true;

            robotTimeLabel = CreateTextLabel(
                "Χρόνος που απομένει: -",
                9.5F,
                TextMuted);
            robotTimeLabel.Location = new Point(230, 156);
            robotTimeLabel.AutoSize = true;

            robotProgressBar = new ProgressBar
            {
                Location = new Point(22, 198),
                Width = 475,
                Height = 25,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            Label detectedTitle = CreateTextLabel(
                "Αναγνωρισμένο αντικείμενο",
                10,
                TextDark);
            detectedTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            detectedTitle.Location = new Point(22, 250);
            detectedTitle.AutoSize = true;

            robotDetectedItemsListBox = new ListBox
            {
                Location = new Point(22, 280),
                Width = 475,
                Height = 115,
                Font = new Font("Segoe UI", 9F)
            };

            robotAlertLabel = CreateTextLabel(
                "Δεν έχουν εντοπιστεί αντικείμενα.",
                9.5F,
                TextMuted);
            robotAlertLabel.Location = new Point(22, 415);
            robotAlertLabel.MaximumSize = new Size(475, 40);
            robotAlertLabel.AutoSize = true;

            statusCard.Controls.Add(robotStatusLabel);
            statusCard.Controls.Add(robotLocationLabel);
            statusCard.Controls.Add(robotLegsLabel);
            statusCard.Controls.Add(robotBatteryLabel);
            statusCard.Controls.Add(robotTimeLabel);
            statusCard.Controls.Add(robotProgressBar);
            statusCard.Controls.Add(detectedTitle);
            statusCard.Controls.Add(robotDetectedItemsListBox);
            statusCard.Controls.Add(robotAlertLabel);

            contentPanel.Controls.Add(optionsCard);
            contentPanel.Controls.Add(statusCard);

            InitializeRobotTimer();
        }

        private void InitializeRobotTimer()
        {
            if (robotTimer != null)
            {
                robotTimer.Stop();
                robotTimer.Dispose();
            }

            robotTimer = new Timer
            {
                Interval = 1000
            };

            robotTimer.Tick += RobotTimer_Tick;
        }

        private void StartRobotCleaning()
        {
            int durationSeconds;

            if (robotDurationComboBox.SelectedIndex == 0)
            {
                durationSeconds = 30;
            }
            else if (robotDurationComboBox.SelectedIndex == 1)
            {
                durationSeconds = 60;
            }
            else
            {
                durationSeconds = 90;
            }

            robotService.StartCleaning(
                robotAreaComboBox.SelectedItem.ToString(),
                robotMethodComboBox.SelectedItem.ToString(),
                GetSelectedCleaningZones(),
                durationSeconds);

            robotIsCleaning = true;

            robotProgressBar.Value = 0;
            robotStatusLabel.Text = "Κατάσταση: Ανάπτυξη ποδιών";
            robotStatusLabel.ForeColor = Blue;
            robotLocationLabel.Text = robotService.GetLocationDescription();
            robotLegsLabel.Text = "Πόδια: Αναπτυγμένα";
            robotBatteryLabel.Text = "Μπαταρία: " +
                                     robotService.State.BatteryLevel + "%";
            robotTimeLabel.Text = "Χρόνος που απομένει: " +
                                  durationSeconds +
                                  " δευτερόλεπτα";
            robotAlertLabel.Text =
                "Οι αισθητήρες καθαρισμού ενεργοποιήθηκαν.";
            robotAlertLabel.ForeColor = TextMuted;

            robotDetectedItemsListBox.Items.Clear();
            robotTimer.Start();

            notificationService.Add(
                "Έναρξη καθαρισμού",
                "Το ρομπότ ξεκίνησε αποστολή στην περιοχή " +
                robotAreaComboBox.SelectedItem + ".",
                NotificationPriority.Normal);
        }

        private void StopRobotCleaning()
        {
            robotService.PauseCleaning();
            robotIsCleaning = false;

            if (robotTimer != null)
            {
                robotTimer.Stop();
            }

            robotStatusLabel.Text = "Κατάσταση: Σε παύση";
            robotStatusLabel.ForeColor = Orange;
            robotAlertLabel.Text = "Η αποστολή βρίσκεται σε παύση.";
            robotAlertLabel.ForeColor = Orange;
        }

        private void RobotTimer_Tick(
            object sender,
            EventArgs e)
        {
            if (!robotIsCleaning)
            {
                return;
            }

            robotService.AdvanceOneSecond();

            RobotState state = robotService.State;

            robotProgressBar.Value = state.Progress;

            robotStatusLabel.Text =
                "Κατάσταση: " +
                GetRobotStatusText(state.Status);

            robotLocationLabel.Text =
                "Θέση: " +
                robotService.GetLocationDescription();

            robotLegsLabel.Text =
                "Πόδια: " +
                (state.LegsExtended
                    ? "Αναπτυγμένα"
                    : "Μαζεμένα");

            robotBatteryLabel.Text =
                "Μπαταρία: " +
                state.BatteryLevel + "%";

            robotTimeLabel.Text =
                "Χρόνος που απομένει: " +
                state.RemainingSeconds +
                " δευτερόλεπτα";

            if (state.Status == RobotStatus.Completed)
            {
                robotIsCleaning = false;
                robotTimer.Stop();

                robotStatusLabel.ForeColor = Green;
                robotAlertLabel.Text =
                    "Ο καθαρισμός ολοκληρώθηκε και το εύρημα καταγράφηκε.";
                robotAlertLabel.ForeColor = Green;
            }
        }

        private string GetRobotStatusText(RobotStatus status)
        {
            switch (status)
            {
                case RobotStatus.DeployingLegs:
                    return "Ανάπτυξη ποδιών";

                case RobotStatus.Moving:
                    return "Μετάβαση στην περιοχή";

                case RobotStatus.Cleaning:
                    return "Καθαρισμός σε εξέλιξη";

                case RobotStatus.DetectingObjects:
                    return "Αναγνώριση αντικειμένων";

                case RobotStatus.Returning:
                    return "Επιστροφή στη βάση";

                case RobotStatus.Paused:
                    return "Σε παύση";

                case RobotStatus.Completed:
                    return "Ολοκληρώθηκε";

                default:
                    return "Σε αναμονή";
            }
        }

        private string GetSelectedCleaningZones()
        {
            if (robotZonesCheckedListBox.CheckedItems.Count == 0)
            {
                return "όλο το επιλεγμένο τμήμα";
            }

            List<string> zones = new List<string>();

            foreach (object item in robotZonesCheckedListBox.CheckedItems)
            {
                zones.Add(item.ToString());
            }

            return string.Join(", ", zones);
        }

        private void RobotService_ObjectDetected(
            object sender,
            DetectedObjectEventArgs e)
        {
            DetectedObject item = e.DetectedObject;

            robotDetectedItemsListBox.Items.Add(
                "[ΥΨΗΛΗ] " +
                item.Time.ToString("HH:mm") +
                " - " +
                item.ObjectName +
                " - " +
                item.Details +
                " Σημείο: " +
                item.Area);

            robotAlertLabel.Text =
                "Εντοπίστηκε αντικείμενο υψηλής σημασίας: " +
                item.ObjectName;
            robotAlertLabel.ForeColor = Red;

            notificationService.AddObjectNotification(item);
        }

        private void RobotService_CleaningCompleted(
            object sender,
            EventArgs e)
        {
            notificationService.Add(
                "Ολοκλήρωση καθαρισμού",
                "Το ρομπότ επέστρεψε στη βάση φόρτισης.",
                NotificationPriority.Normal);
        }

        private void ShowNotificationsPage()
        {
            PreparePage(
                "Ειδοποιήσεις",
                "Ενημερώσεις για τον οδηγό, την εταιρεία και τους επιβάτες.");

            Panel notificationsCard = CreateCard(0, 100, 980, 520);
            AddCardTitle(notificationsCard, "Καταγεγραμμένες ειδοποιήσεις");

            IReadOnlyList<NotificationItem> items =
                notificationService.GetAll();

            if (items.Count == 0)
            {
                Label empty = CreateTextLabel(
                    "Δεν υπάρχουν ειδοποιήσεις.",
                    10,
                    TextMuted);
                empty.Location = new Point(25, 70);
                empty.AutoSize = true;
                notificationsCard.Controls.Add(empty);
            }
            else
            {
                int y = 65;

                foreach (NotificationItem item in items)
                {
                    Panel row = new Panel
                    {
                        Width = 900,
                        Height = 72,
                        Location = new Point(22, y),
                        BackColor =
                            item.Priority == NotificationPriority.High
                                ? Color.FromArgb(253, 242, 242)
                                : LightBlue,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Label title = CreateTextLabel(
                        item.Title,
                        10,
                        item.Priority == NotificationPriority.High
                            ? Red
                            : TextDark);
                    title.Font = new Font(
                        "Segoe UI",
                        10F,
                        FontStyle.Bold);
                    title.Location = new Point(12, 9);
                    title.AutoSize = true;

                    Label message = CreateTextLabel(
                        item.Message,
                        9,
                        TextMuted);
                    message.Location = new Point(12, 37);
                    message.MaximumSize = new Size(700, 25);
                    message.AutoSize = true;

                    Label time = CreateTextLabel(
                        item.Time.ToString("HH:mm"),
                        9,
                        TextMuted);
                    time.Location = new Point(820, 25);
                    time.AutoSize = true;

                    row.Controls.Add(title);
                    row.Controls.Add(message);
                    row.Controls.Add(time);
                    notificationsCard.Controls.Add(row);

                    y += 83;

                    if (y > 475)
                    {
                        break;
                    }
                }
            }

            Button clearButton = CreateSecondaryButton(
                "Καθαρισμός ειδοποιήσεων");
            clearButton.Location = new Point(22, 555);
            clearButton.Size = new Size(190, 35);

            clearButton.Click += delegate
            {
                notificationService.Clear();
                ShowNotificationsPage();
            };

            contentPanel.Controls.Add(notificationsCard);
            contentPanel.Controls.Add(clearButton);
        }

        private void ShowNavigationPage()
        {
            PreparePage(
                "Τουριστική πλοήγηση",
                "Βρείτε αξιοθέατα και την κοντινότερη στάση.");

            Panel navigationCard = CreateCard(0, 100, 980, 430);
            AddCardTitle(navigationCard, "Προορισμός");

            ComboBox destination = new ComboBox
            {
                Location = new Point(25, 75),
                Width = 360,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            destination.Items.Add("Ακρόπολη");
            destination.Items.Add("Μουσείο Ακρόπολης");
            destination.Items.Add("Εθνικός Κήπος");
            destination.Items.Add("Κοντινότερη στάση λεωφορείου");
            destination.SelectedIndex = 0;

            Button startButton = CreatePrimaryButton(
                "Έναρξη πλοήγησης");
            startButton.Location = new Point(25, 130);
            startButton.Size = new Size(180, 40);

            Label gpsStatus = CreateTextLabel(
                "GPS: Ενεργό  •  Σύνδεση με το λεωφορείο: Ενεργή",
                10,
                Green);
            gpsStatus.Location = new Point(25, 205);
            gpsStatus.AutoSize = true;

            Panel map = new Panel
            {
                Location = new Point(25, 250),
                Size = new Size(900, 120),
                BackColor = LightBlue
            };

            Label mapText = CreateTextLabel(
                "Χάρτης περιοχής\n\n" +
                "Η τρέχουσα θέση βρίσκεται κοντά στη στάση Ακρόπολη.",
                11,
                Blue);
            mapText.Dock = DockStyle.Fill;
            mapText.TextAlign = ContentAlignment.MiddleCenter;

            startButton.Click += delegate
            {
                MessageBox.Show(
                    "Η πλοήγηση προς «" +
                    destination.SelectedItem +
                    "» ξεκίνησε.",
                    "Τουριστικό GPS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            map.Controls.Add(mapText);
            navigationCard.Controls.Add(destination);
            navigationCard.Controls.Add(startButton);
            navigationCard.Controls.Add(gpsStatus);
            navigationCard.Controls.Add(map);

            contentPanel.Controls.Add(navigationCard);
        }

        private void ShowHelpPage()
        {
            PreparePage(
                "Βοήθεια",
                "Οδηγίες για τις βασικές λειτουργίες του SmartBuss.");

            Panel helpCard = CreateCard(0, 100, 980, 510);
            AddCardTitle(helpCard, "Χρήση της εφαρμογής");

            RichTextBox helpText = new RichTextBox
            {
                Location = new Point(22, 60),
                Size = new Size(925, 420),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = TextDark,
                Font = new Font("Segoe UI", 10.5F),
                Text =
                    "Επιβάτης\n\n" +
                    "Το λεωφορείο διαθέτει κάτω και πάνω όροφο.\n" +
                    "Από τη «Διαδρομή» ενημερώνεστε για τα αξιοθέατα.\n" +
                    "Από τις «Παραγγελίες» επιλέγετε προϊόντα από συνεργαζόμενα καταστήματα.\n" +
                    "Η πληρωμή γίνεται με εικονική κάρτα και η παραγγελία\n" +
                    "παραδίδεται σε επόμενη στάση.\n\n" +
                    "Κατάσταση παραγγελίας\n\n" +
                    "Η κατάσταση αλλάζει αυτόματα κάθε 10 δευτερόλεπτα:\n" +
                    "«Σε επεξεργασία» → «Προς παράδοση» → «Παραδόθηκε».\n\n" +
                    "Ρομπότ καθαρισμού\n\n" +
                    "Ο τεχνικός επιλέγει κάτω όροφο, πάνω όροφο, περιοχή οδηγού,\n" +
                    "σκάλα ή όλο το λεωφορείο. Επιλέγει επίσης σημεία καθαρισμού,\n" +
                    "μέθοδο και χρόνο ολοκλήρωσης.\n" +
                    "Το ρομπότ αναπτύσσει τα πόδια του, μετακινείται και καθαρίζει.\n" +
                    "Σε κάθε αποστολή μπορεί να αναγνωρίσει ένα αντικείμενο αξίας.\n\n" +
                    "Οι ειδοποιήσεις ενημερώνουν τον οδηγό, την εταιρεία και τους επιβάτες.\n\n" +
                    "Σημείωση: Η εφαρμογή είναι εκπαιδευτική προσομοίωση."
            };

            helpCard.Controls.Add(helpText);
            contentPanel.Controls.Add(helpCard);
        }

        private void StartTimers()
        {
            clockTimer = new Timer
            {
                Interval = 1000
            };

            clockTimer.Tick += delegate
            {
                clockLabel.Text = DateTime.Now.ToString("HH:mm");
            };

            clockTimer.Start();

            simulationTimer = new Timer
            {
                Interval = 5000
            };

            simulationTimer.Tick += delegate
            {
                currentSpeed = random.Next(32, 54);
                solarEfficiency = random.Next(62, 94);

                batteryLevel = Math.Max(
                    35,
                    Math.Min(
                        100,
                        batteryLevel + random.Next(-2, 4)));

                connectionLabel.Text = "● Συνδεδεμένο";
                connectionLabel.ForeColor =
                    Color.FromArgb(152, 221, 174);
            };

            simulationTimer.Start();

            orderTimer = new Timer
            {
                Interval = 10000
            };

            orderTimer.Tick += delegate
            {
                List<OrderStatusChange> changes =
                    orderService.AdvanceOrders();

                foreach (OrderStatusChange change in changes)
                {
                    if (change.NewStatus == "Προς παράδοση")
                    {
                        notificationService.Add(
                            "Παραγγελία #" +
                            change.Order.Number,
                            "Η παραγγελία είναι έτοιμη και κατευθύνεται " +
                            "προς τη στάση " +
                            change.Order.DeliveryStop + ".",
                            NotificationPriority.Normal);
                    }
                    else if (change.NewStatus == "Παραδόθηκε")
                    {
                        notificationService.Add(
                            "Παραγγελία #" +
                            change.Order.Number,
                            "Η παραγγελία παραδόθηκε στη στάση " +
                            change.Order.DeliveryStop + ".",
                            NotificationPriority.Normal);
                    }
                }
            };

            orderTimer.Start();
        }

        private Panel CreateCard(
            int x,
            int y,
            int width,
            int height)
        {
            return new Panel
            {
                Location = new Point(x + 28, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void AddCardTitle(
            Panel card,
            string title)
        {
            Label label = new Label
            {
                Text = title,
                AutoSize = true,
                ForeColor = TextDark,
                Font = new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold),
                Location = new Point(20, 18)
            };

            card.Controls.Add(label);
        }

        private Label CreateTextLabel(
            string text,
            float size,
            Color color)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size),
                ForeColor = color
            };
        }

        private Label CreateMetricLabel(
            string text,
            Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    26F,
                    FontStyle.Bold),
                ForeColor = color
            };
        }

        private Button CreatePrimaryButton(
            string text)
        {
            Button button = new Button
            {
                Text = text,
                BackColor = Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private Button CreateSecondaryButton(
            string text)
        {
            Button button = new Button
            {
                Text = text,
                BackColor = Color.White,
                ForeColor = TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.BorderSize = 1;

            return button;
        }

        private void AddActionButton(
            Panel parent,
            string title,
            string description,
            int x,
            int y,
            EventHandler action)
        {
            Button button = CreateSecondaryButton(
                title + "\n" + description);

            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(12, 0, 5, 0);
            button.Location = new Point(x, y);
            button.Size = new Size(430, 48);
            button.Click += action;

            parent.Controls.Add(button);
        }

        private Color GetStatusColor(string status)
        {
            if (status == "Παραδόθηκε")
            {
                return Green;
            }

            if (status == "Προς παράδοση")
            {
                return Blue;
            }

            if (status == "Σε επεξεργασία")
            {
                return Orange;
            }

            return TextDark;
        }
    }
}
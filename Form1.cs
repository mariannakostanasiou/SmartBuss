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
        private Timer robotTimer;
        private Timer orderTimer;

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

        private decimal cartTotal = 0;
        private int nextOrderNumber = 1001;

        private readonly List<CartItem> cartItems = new List<CartItem>();
        private readonly List<Order> orders = new List<Order>();
        private readonly List<DetectedObject> detectedObjects =
            new List<DetectedObject>();
        private readonly List<NotificationItem> notifications =
            new List<NotificationItem>();

        private bool robotIsCleaning;
        private bool robotLegsExtended;
        private int robotProgress;
        private int robotStep;
        private int robotDurationSeconds;

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

        private int orderRefreshCounter;

        public Form1()
        {
            InitializeComponent();

            ConfigureForm();
            BuildApplication();
            StartTimers();
            ShowPassengerDashboard();
        }

        private void ConfigureForm()
        {
            BackColor = Background;
            Font = new Font("Segoe UI", 10F);
            Text = "SmartBuss - Έξυπνο Τουριστικό Λεωφορείο";
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

            Label brandLabel = new Label
            {
                Text = "SmartBuss",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Location = new Point(25, 13)
            };

            Label subtitleLabel = new Label
            {
                Text = "Έξυπνο τουριστικό λεωφορείο",
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
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(970, 20)
            };

            clockLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(1145, 26)
            };

            header.Resize += delegate
            {
                connectionLabel.Left = header.Width - 290;
                clockLabel.Left = header.Width - 105;
            };

            header.Controls.Add(brandLabel);
            header.Controls.Add(subtitleLabel);
            header.Controls.Add(connectionLabel);
            header.Controls.Add(clockLabel);

            return header;
        }

        private Panel BuildSidebar()
        {
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 210,
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

            Button passengerButton = CreateMenuButton("Επισκόπηση");
            Button routeButton = CreateMenuButton("Διαδρομή");
            Button ordersButton = CreateMenuButton("Παραγγελίες");
            Button trackingButton = CreateMenuButton("Παρακολούθηση");
            Button cafeButton = CreateMenuButton("Οθόνη καφετέριας");
            Button driverButton = CreateMenuButton("Οθόνη οδηγού");
            Button employeeButton = CreateMenuButton("Τεχνικός έλεγχος");
            Button robotButton = CreateMenuButton("Ρομπότ καθαρισμού");
            Button notificationsButton = CreateMenuButton("Ειδοποιήσεις");
            Button helpButton = CreateMenuButton("Βοήθεια");

            int y = 55;

            Button[] buttons =
            {
                passengerButton,
                routeButton,
                ordersButton,
                trackingButton,
                cafeButton,
                driverButton,
                employeeButton,
                robotButton,
                notificationsButton,
                helpButton
            };

            foreach (Button button in buttons)
            {
                button.Location = new Point(14, y);
                y += 42;
            }

            passengerButton.Click += delegate { ShowPassengerDashboard(); };
            routeButton.Click += delegate { ShowRoutePage(); };
            ordersButton.Click += delegate { ShowOrderPage(); };
            trackingButton.Click += delegate { ShowOrderTrackingPage(); };
            cafeButton.Click += delegate { ShowCafePage(); };
            driverButton.Click += delegate { ShowDriverPage(); };
            employeeButton.Click += delegate { ShowEmployeePage(); };
            robotButton.Click += delegate { ShowRobotPage(); };
            notificationsButton.Click += delegate { ShowNotificationsPage(); };
            helpButton.Click += delegate { ShowHelpPage(); };

            Panel separator = new Panel
            {
                Height = 1,
                Width = 180,
                BackColor = Border,
                Location = new Point(14, 520)
            };

            Label busInfo = new Label
            {
                Text = "Λεωφορείο 405\nΓραμμή: Downtown Express\nΤρέχουσα στάση: Ακρόπολη",
                AutoSize = true,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(18, 540)
            };

            sidebar.Controls.Add(menuLabel);

            foreach (Button button in buttons)
            {
                sidebar.Controls.Add(button);
            }

            sidebar.Controls.Add(separator);
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
                "Η τρέχουσα κατάσταση του λεωφορείου και οι διαθέσιμες υπηρεσίες.");

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
                "Σύνταγμα  →  Ακρόπολη  →  Μουσείο\n\n" +
                "Μουσείο  →  Εθνικός Κήπος  →  Παραλία",
                11,
                Blue);
            routeMap.TextAlign = ContentAlignment.MiddleCenter;
            routeMap.BackColor = LightBlue;
            routeMap.Location = new Point(20, 58);
            routeMap.Size = new Size(430, 115);

            Label routeInfo = CreateTextLabel(
                "Μπορείτε να αποβιβαστείτε και να επιστρέψετε " +
                "με το επόμενο λεωφορείο της ίδιας γραμμής.",
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
                "Η θερμοκρασία και η διαδρομή λειτουργούν κανονικά.",
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
            AddCardTitle(routeCard, "Η διαδρομή του λεωφορείου");

            Label route = CreateTextLabel(
                "Σύνταγμα  →  Ακρόπολη  →  Μουσείο  →  Εθνικός Κήπος  →  Παραλία",
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
                "Επιλέξτε προϊόντα και ορίστε τη στάση παράδοσης.");

            Panel storesCard = CreateCard(0, 100, 620, 540);
            AddCardTitle(storesCard, "Μενού συνεργαζόμενων καταστημάτων");

            AddProductButton(
                storesCard, "Espresso", "Coffee & Café",
                2.50m, 25, 65);

            AddProductButton(
                storesCard, "Cappuccino", "Coffee & Café",
                3.20m, 25, 125);

            AddProductButton(
                storesCard, "Κρουασάν", "Coffee & Café",
                2.80m, 25, 185);

            AddProductButton(
                storesCard, "Burger", "Fast Food Central",
                6.50m, 315, 65);

            AddProductButton(
                storesCard, "Pizza slice", "Fast Food Central",
                4.50m, 315, 125);

            AddProductButton(
                storesCard, "Αναψυκτικό", "Fast Food Central",
                2.20m, 315, 185);

            AddProductButton(
                storesCard, "Chicken wrap", "Healthy Hub",
                6.80m, 25, 275);

            AddProductButton(
                storesCard, "Smoothie", "Healthy Hub",
                4.20m, 315, 275);

            Label note = CreateTextLabel(
                "Τα καταστήματα ενημερώνονται αυτόματα όταν ολοκληρώσετε την παραγγελία.",
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

            Button checkoutButton = CreatePrimaryButton(
                "Συνέχεια στην πληρωμή");
            checkoutButton.Location = new Point(20, 410);
            checkoutButton.Size = new Size(290, 42);

            checkoutButton.Click += delegate
            {
                CheckoutOrder(itemsPanel, totalLabel);
            };

            Button clearButton = CreateSecondaryButton(
                "Καθαρισμός καλαθιού");
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
                        (item.Price * item.Quantity).ToString("0.00") + " €",
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

            Order order = new Order
            {
                Number = nextOrderNumber++,
                Store = cartItems[0].Store,
                DeliveryStop = deliveryStop,
                Status = "Νέα παραγγελία",
                CreatedAt = DateTime.Now,
                Total = cartTotal,
                Items = cartItems
                    .Select(item => item.Name + " x" + item.Quantity)
                    .ToList()
            };

            orders.Add(order);

            AddNotification(
                "Νέα παραγγελία #" + order.Number,
                "Η παραγγελία στάλθηκε στη " + order.Store + ".",
                NotificationPriority.Normal);

            MessageBox.Show(
                "Η παραγγελία #" + order.Number +
                " καταχωρήθηκε επιτυχώς.\n\n" +
                "Κατάστημα: " + order.Store + "\n" +
                "Παράδοση: στάση " + order.DeliveryStop + "\n" +
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

                return paymentForm.ShowDialog(this) == DialogResult.OK;
            }
        }

        private string SelectDeliveryStop()
        {
            using (Form stopForm = new Form())
            {
                stopForm.Text = "Στάση παράδοσης";
                stopForm.Size = new Size(400, 245);
                stopForm.StartPosition = FormStartPosition.CenterParent;
                stopForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                stopForm.MaximizeBox = false;
                stopForm.MinimizeBox = false;

                Label description = new Label
                {
                    Text = "Επιλέξτε την επόμενη στάση όπου θα παραδοθεί η παραγγελία.",
                    AutoSize = false,
                    Width = 330,
                    Height = 42,
                    Location = new Point(25, 22)
                };

                ComboBox stops = new ComboBox
                {
                    Location = new Point(25, 82),
                    Width = 330,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                stops.Items.Add("Ακρόπολη");
                stops.Items.Add("Μουσείο");
                stops.Items.Add("Εθνικός Κήπος");
                stops.Items.Add("Παραλιακή Ζώνη");
                stops.SelectedIndex = 0;

                Button okButton = CreatePrimaryButton("Επιβεβαίωση");
                okButton.Location = new Point(25, 140);
                okButton.Size = new Size(145, 38);

                Button cancelButton = CreateSecondaryButton("Άκυρο");
                cancelButton.Location = new Point(205, 140);
                cancelButton.Size = new Size(145, 38);

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
                "Δείτε την κατάσταση των παραγγελιών που έχουν καταχωρηθεί.");

            Panel ordersCard = CreateCard(0, 100, 980, 500);
            AddCardTitle(ordersCard, "Οι παραγγελίες σας");

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
                    orderLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
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
                }
            }

            contentPanel.Controls.Add(ordersCard);
        }

        private void ShowCafePage()
        {
            PreparePage(
                "Οθόνη συνεργαζόμενης καφετέριας",
                "Ο υπάλληλος βλέπει και διαχειρίζεται τις παραγγελίες του λεωφορείου.");

            Panel cafeCard = CreateCard(0, 100, 980, 535);
            AddCardTitle(cafeCard, "Εισερχόμενες παραγγελίες");

            if (orders.Count == 0)
            {
                Label empty = CreateTextLabel(
                    "Δεν υπάρχουν νέες παραγγελίες.",
                    10,
                    TextMuted);
                empty.Location = new Point(25, 70);
                empty.AutoSize = true;
                cafeCard.Controls.Add(empty);
            }
            else
            {
                int y = 65;

                foreach (Order order in orders)
                {
                    Panel row = new Panel
                    {
                        Width = 900,
                        Height = 120,
                        Location = new Point(22, y),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Label title = CreateTextLabel(
                        "Παραγγελία #" + order.Number +
                        "  •  " + order.Store,
                        10.5F,
                        TextDark);
                    title.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                    title.Location = new Point(12, 10);
                    title.AutoSize = true;

                    Label items = CreateTextLabel(
                        "Προϊόντα: " + string.Join(", ", order.Items),
                        8.8F,
                        TextMuted);
                    items.Location = new Point(12, 38);
                    items.MaximumSize = new Size(560, 35);
                    items.AutoSize = true;

                    Label stop = CreateTextLabel(
                        "Παράδοση στη στάση: " + order.DeliveryStop,
                        9.5F,
                        Blue);
                    stop.Location = new Point(12, 80);
                    stop.AutoSize = true;

                    ComboBox statusBox = new ComboBox
                    {
                        Location = new Point(650, 18),
                        Width = 210,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    statusBox.Items.Add("Νέα παραγγελία");
                    statusBox.Items.Add("Σε προετοιμασία");
                    statusBox.Items.Add("Έτοιμη");
                    statusBox.Items.Add("Καθ’ οδόν");
                    statusBox.Items.Add("Παραδόθηκε");

                    int selectedIndex = statusBox.Items.IndexOf(order.Status);
                    statusBox.SelectedIndex = selectedIndex >= 0
                        ? selectedIndex
                        : 0;

                    Button updateButton = CreatePrimaryButton("Ενημέρωση");
                    updateButton.Location = new Point(650, 65);
                    updateButton.Size = new Size(125, 34);

                    updateButton.Click += delegate
                    {
                        order.Status = statusBox.SelectedItem.ToString();

                        AddNotification(
                            "Ενημέρωση παραγγελίας #" + order.Number,
                            "Νέα κατάσταση: " + order.Status,
                            NotificationPriority.Normal);

                        MessageBox.Show(
                            "Η κατάσταση της παραγγελίας ενημερώθηκε.",
                            "Καφετέρια",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    };

                    row.Controls.Add(title);
                    row.Controls.Add(items);
                    row.Controls.Add(stop);
                    row.Controls.Add(statusBox);
                    row.Controls.Add(updateButton);
                    cafeCard.Controls.Add(row);

                    y += 135;
                }
            }

            contentPanel.Controls.Add(cafeCard);
        }

        private void ShowDriverPage()
        {
            PreparePage(
                "Πίνακας οδηγού",
                "Παρακολούθηση πορείας, ασφάλειας και ενημερώσεων.");

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
            AddCardTitle(safetyCard, "Ασφάλεια");

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

            Panel alertsCard = CreateCard(0, 315, 980, 150);
            AddCardTitle(alertsCard, "Ειδοποιήσεις οδήγησης");

            Label alerts = CreateTextLabel(
                currentSpeed > 50
                    ? "ΠΡΟΕΙΔΟΠΟΙΗΣΗ: Υπέρβαση ορίου ταχύτητας."
                    : "Δεν υπάρχουν ενεργές προειδοποιήσεις.",
                currentSpeed > 50 ? 10 : 10,
                currentSpeed > 50 ? Red : Green);
            alerts.Location = new Point(22, 62);
            alerts.AutoSize = true;

            CheckBox laneCheck = new CheckBox
            {
                Text = "Παρακολούθηση λωρίδας",
                Checked = true,
                AutoSize = true,
                Location = new Point(22, 95)
            };

            CheckBox fatigueCheck = new CheckBox
            {
                Text = "Έλεγχος κόπωσης οδηγού",
                Checked = true,
                AutoSize = true,
                Location = new Point(230, 95)
            };

            alertsCard.Controls.Add(alerts);
            alertsCard.Controls.Add(laneCheck);
            alertsCard.Controls.Add(fatigueCheck);

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

            Panel roofCard = CreateCard(330, 100, 310, 220);
            AddCardTitle(roofCard, "Ρυθμιζόμενη οροφή");

            Label roofStatus = CreateTextLabel(
                "Κατάσταση: Ανοιχτή",
                11,
                Green);
            roofStatus.Location = new Point(22, 65);
            roofStatus.AutoSize = true;

            Button openButton = CreatePrimaryButton("Άνοιγμα");
            openButton.Location = new Point(22, 120);
            openButton.Size = new Size(110, 38);

            Button closeButton = CreateSecondaryButton("Κλείσιμο");
            closeButton.Location = new Point(145, 120);
            closeButton.Size = new Size(110, 38);

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

            roofCard.Controls.Add(roofStatus);
            roofCard.Controls.Add(openButton);
            roofCard.Controls.Add(closeButton);

            Panel systemCard = CreateCard(660, 100, 320, 220);
            AddCardTitle(systemCard, "Συστήματα");

            Label systemStatus = CreateTextLabel(
                "Κλιματισμός: Ενεργός\nΦωτισμός: Ενεργός\nΣύνδεση αισθητήρων: Ενεργή",
                10,
                Green);
            systemStatus.Location = new Point(22, 65);
            systemStatus.AutoSize = true;

            systemCard.Controls.Add(systemStatus);

            Panel reportCard = CreateCard(0, 350, 980, 145);
            AddCardTitle(reportCard, "Τελευταίος τεχνικός έλεγχος");

            Label report = CreateTextLabel(
                "Όλα τα βασικά συστήματα λειτουργούν κανονικά.\n" +
                "Τελευταία ενημέρωση: πριν από 2 λεπτά.",
                10,
                TextMuted);
            report.Location = new Point(22, 62);
            report.AutoSize = true;

            reportCard.Controls.Add(report);

            contentPanel.Controls.Add(energyCard);
            contentPanel.Controls.Add(roofCard);
            contentPanel.Controls.Add(systemCard);
            contentPanel.Controls.Add(reportCard);
        }

        private void ShowRobotPage()
        {
            PreparePage(
                "Ρομπότ καθαρισμού",
                "Επιλέξτε περιοχή, μέθοδο και διάρκεια καθαρισμού.");

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
            robotAreaComboBox.Items.Add("Ισόγειο");
            robotAreaComboBox.Items.Add("1ος όροφος");
            robotAreaComboBox.Items.Add("2ος όροφος");
            robotAreaComboBox.Items.Add("Περιοχή οδηγού");
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

            robotZonesCheckedListBox.Items.Add("Καθίσματα");
            robotZonesCheckedListBox.Items.Add("Διάδρομοι");
            robotZonesCheckedListBox.Items.Add("Σκαλοπάτια");
            robotZonesCheckedListBox.Items.Add("Περιοχή οδηγού");

            Button startButton = CreatePrimaryButton(
                "Έναρξη αποστολής");
            startButton.Location = new Point(22, 425);
            startButton.Size = new Size(160, 40);

            Button stopButton = CreateSecondaryButton(
                "Παύση");
            stopButton.Location = new Point(195, 425);
            stopButton.Size = new Size(105, 40);

            startButton.Click += delegate
            {
                StartRobotCleaning();
            };

            stopButton.Click += delegate
            {
                StopRobotCleaning();
            };

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
                "Αναγνωρισμένα αντικείμενα",
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

            InitializeRobotControls();
        }

        private void InitializeRobotControls()
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
            if (robotAreaComboBox == null ||
                robotMethodComboBox == null ||
                robotDurationComboBox == null ||
                robotProgressBar == null)
            {
                return;
            }

            robotIsCleaning = true;
            robotLegsExtended = true;
            robotProgress = 0;
            robotStep = 0;

            if (robotDurationComboBox.SelectedIndex == 0)
            {
                robotDurationSeconds = 30;
            }
            else if (robotDurationComboBox.SelectedIndex == 1)
            {
                robotDurationSeconds = 60;
            }
            else
            {
                robotDurationSeconds = 90;
            }

            robotProgressBar.Value = 0;

            robotStatusLabel.Text = "Κατάσταση: Ανάπτυξη ποδιών";
            robotStatusLabel.ForeColor = Blue;

            robotLocationLabel.Text =
                "Θέση: Είσοδος λεωφορείου";

            robotLegsLabel.Text =
                "Πόδια: Αναπτυγμένα";

            robotBatteryLabel.Text =
                "Μπαταρία: " + batteryLevel + "%";

            robotTimeLabel.Text =
                "Χρόνος που απομένει: " +
                robotDurationSeconds + " δευτερόλεπτα";

            robotAlertLabel.Text =
                "Οι αισθητήρες καθαρισμού ενεργοποιήθηκαν.";

            robotTimer.Start();

            AddNotification(
                "Έναρξη καθαρισμού",
                "Το ρομπότ ξεκίνησε αποστολή στην περιοχή " +
                robotAreaComboBox.SelectedItem + ".",
                NotificationPriority.Normal);
        }

        private void StopRobotCleaning()
        {
            if (robotTimer != null)
            {
                robotTimer.Stop();
            }

            robotIsCleaning = false;

            if (robotStatusLabel != null)
            {
                robotStatusLabel.Text = "Κατάσταση: Σε παύση";
                robotStatusLabel.ForeColor = Orange;
            }

            if (robotLegsLabel != null)
            {
                robotLegsLabel.Text =
                    "Πόδια: " +
                    (robotLegsExtended ? "Αναπτυγμένα" : "Μαζεμένα");
            }

            if (robotAlertLabel != null)
            {
                robotAlertLabel.Text =
                    "Η αποστολή βρίσκεται σε παύση.";
            }
        }

        private void RobotTimer_Tick(object sender, EventArgs e)
        {
            if (!robotIsCleaning)
            {
                return;
            }

            robotStep++;

            robotProgress = Math.Min(
                100,
                robotProgress + 5);

            robotProgressBar.Value = robotProgress;

            int remainingSeconds = Math.Max(
                0,
                robotDurationSeconds - robotStep);

            robotTimeLabel.Text =
                "Χρόνος που απομένει: " +
                remainingSeconds + " δευτερόλεπτα";

            if (robotProgress <= 10)
            {
                robotStatusLabel.Text =
                    "Κατάσταση: Ανάπτυξη ποδιών";

                robotLocationLabel.Text =
                    "Θέση: Είσοδος λεωφορείου";
            }
            else if (robotProgress <= 30)
            {
                robotStatusLabel.Text =
                    "Κατάσταση: Μετάβαση στην περιοχή";

                robotLocationLabel.Text =
                    "Θέση: " + robotAreaComboBox.SelectedItem;
            }
            else if (robotProgress <= 65)
            {
                robotStatusLabel.Text =
                    "Κατάσταση: Καθαρισμός σε εξέλιξη";

                robotLocationLabel.Text =
                    "Περιοχή: " +
                    GetSelectedCleaningZones();
            }
            else if (robotProgress <= 90)
            {
                robotStatusLabel.Text =
                    "Κατάσταση: Αναγνώριση αντικειμένων";

                robotLocationLabel.Text =
                    "Θέση: Τελικός έλεγχος χώρου";
            }
            else
            {
                robotStatusLabel.Text =
                    "Κατάσταση: Επιστροφή στη βάση";

                robotLocationLabel.Text =
                    "Θέση: Σταθμός φόρτισης";
            }

            if (robotProgress == 35)
            {
                AddDetectedObject(
                    "Διαβατήριο",
                    "Εντοπίστηκε κάτω από κάθισμα.",
                    NotificationPriority.High);
            }

            if (robotProgress == 55)
            {
                AddDetectedObject(
                    "Κόσμημα",
                    "Εντοπίστηκε στον διάδρομο.",
                    NotificationPriority.High);
            }

            if (robotProgress == 75)
            {
                AddDetectedObject(
                    "Χαρτονομίσματα",
                    "Εντοπίστηκαν κοντά στη θέση 12A.",
                    NotificationPriority.High);
            }

            if (robotProgress >= 100)
            {
                robotTimer.Stop();
                robotIsCleaning = false;
                robotLegsExtended = false;

                robotStatusLabel.Text =
                    "Κατάσταση: Ολοκληρώθηκε";
                robotStatusLabel.ForeColor = Green;

                robotLocationLabel.Text =
                    "Θέση: Σταθμός φόρτισης";

                robotLegsLabel.Text =
                    "Πόδια: Μαζεμένα";

                robotTimeLabel.Text =
                    "Χρόνος που απομένει: 0 δευτερόλεπτα";

                robotAlertLabel.Text =
                    "Ο καθαρισμός ολοκληρώθηκε και τα ευρήματα καταγράφηκαν.";

                AddNotification(
                    "Ολοκλήρωση καθαρισμού",
                    "Το ρομπότ επέστρεψε στη βάση φόρτισης.",
                    NotificationPriority.Normal);
            }
        }

        private string GetSelectedCleaningZones()
        {
            if (robotZonesCheckedListBox == null ||
                robotZonesCheckedListBox.CheckedItems.Count == 0)
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

        private void AddDetectedObject(
            string objectName,
            string details,
            NotificationPriority priority)
        {
            DetectedObject detectedObject = new DetectedObject
            {
                ObjectName = objectName,
                Details = details,
                Floor = robotAreaComboBox == null
                    ? "Άγνωστος χώρος"
                    : robotAreaComboBox.SelectedItem.ToString(),
                Time = DateTime.Now
            };

            detectedObjects.Add(detectedObject);

            if (robotDetectedItemsListBox != null)
            {
                string entry =
                    "[" + GetPriorityText(priority) + "] " +
                    detectedObject.Time.ToString("HH:mm") +
                    " - " +
                    objectName +
                    " - " +
                    details +
                    " Περιοχή: " +
                    detectedObject.Floor;

                robotDetectedItemsListBox.Items.Add(entry);
            }

            if (robotAlertLabel != null)
            {
                robotAlertLabel.Text =
                    "Εντοπίστηκε αντικείμενο υψηλής σημασίας: " +
                    objectName;
                robotAlertLabel.ForeColor = Red;
            }

            AddNotification(
                "Εντοπίστηκε " + objectName,
                details + " Περιοχή: " + detectedObject.Floor,
                priority);
        }

        private void ShowNotificationsPage()
        {
            PreparePage(
                "Ειδοποιήσεις",
                "Ενημερώσεις για τον οδηγό, την εταιρεία και τους επιβάτες.");

            Panel notificationsCard = CreateCard(0, 100, 980, 520);
            AddCardTitle(notificationsCard, "Καταγεγραμμένες ειδοποιήσεις");

            if (notifications.Count == 0)
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

                foreach (NotificationItem item in notifications
                    .OrderByDescending(notification => notification.Time))
                {
                    Panel row = new Panel
                    {
                        Width = 900,
                        Height = 72,
                        Location = new Point(22, y),
                        BackColor = item.Priority == NotificationPriority.High
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
                    title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
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
                notifications.Clear();
                ShowNotificationsPage();
            };

            contentPanel.Controls.Add(notificationsCard);
            contentPanel.Controls.Add(clearButton);
        }

        private void AddNotification(
            string title,
            string message,
            NotificationPriority priority)
        {
            notifications.Add(new NotificationItem
            {
                Title = title,
                Message = message,
                Priority = priority,
                Time = DateTime.Now
            });
        }

        private void ShowNavigationPage()
        {
            PreparePage(
                "Τουριστική πλοήγηση",
                "Βρείτε αξιοθέατα, εστιατόρια και την κοντινότερη στάση.");

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
                "Η τρέχουσα θέση σας βρίσκεται κοντά στη στάση Ακρόπολη.",
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
                    "Από την «Επισκόπηση» βλέπετε την τρέχουσα διαδρομή.\n" +
                    "Από τη «Διαδρομή» ενημερώνεστε για τα αξιοθέατα.\n" +
                    "Από τις «Παραγγελίες» επιλέγετε προϊόντα από συνεργαζόμενες καφετέριες.\n" +
                    "Η παραγγελία πληρώνεται με εικονική κάρτα και παραδίδεται σε στάση.\n\n" +
                    "Καφετέρια\n\n" +
                    "Ο υπάλληλος της καφετέριας βλέπει τις παραγγελίες και ενημερώνει\n" +
                    "την κατάστασή τους από «Νέα» έως «Παραδόθηκε».\n\n" +
                    "Ρομπότ καθαρισμού\n\n" +
                    "Ο τεχνικός επιλέγει περιοχή, σημεία καθαρισμού, μέθοδο και χρόνο.\n" +
                    "Το ρομπότ αναπτύσσει τα πόδια του, μετακινείται, καθαρίζει και\n" +
                    "αναγνωρίζει αντικείμενα όπως διαβατήριο, κόσμημα και χρήματα.\n\n" +
                    "Τα αναγνωρισμένα αντικείμενα εμφανίζονται στις «Ειδοποιήσεις».\n" +
                    "Η λειτουργία ενημέρωσης οδηγού, εταιρείας και επιβατών\n" +
                    "προσομοιώνεται μέσα στην εφαρμογή.\n\n" +
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
                if (clockLabel != null)
                {
                    clockLabel.Text = DateTime.Now.ToString("HH:mm");
                }
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
                    Math.Min(100, batteryLevel + random.Next(-2, 4)));

                if (connectionLabel != null)
                {
                    connectionLabel.Text = "● Συνδεδεμένο";
                    connectionLabel.ForeColor =
                        Color.FromArgb(152, 221, 174);
                }
            };

            simulationTimer.Start();

            orderTimer = new Timer
            {
                Interval = 8000
            };

            orderTimer.Tick += delegate
            {
                orderRefreshCounter++;

                if (orderRefreshCounter % 3 == 0)
                {
                    Order order = orders.FirstOrDefault(
                        item => item.Status == "Νέα παραγγελία");

                    if (order != null)
                    {
                        order.Status = "Σε προετοιμασία";

                        AddNotification(
                            "Η καφετέρια ξεκίνησε την παραγγελία #" +
                            order.Number,
                            "Η παραγγελία βρίσκεται σε προετοιμασία.",
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
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
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

        private Color GetStatusColor(
            string status)
        {
            if (status == "Παραδόθηκε")
            {
                return Green;
            }

            if (status == "Καθ’ οδόν")
            {
                return Blue;
            }

            if (status == "Έτοιμη")
            {
                return Orange;
            }

            return TextDark;
        }

        private string GetPriorityText(
            NotificationPriority priority)
        {
            if (priority == NotificationPriority.High)
            {
                return "ΥΨΗΛΗ";
            }

            return "ΚΑΝΟΝΙΚΗ";
        }

        private class CartItem
        {
            public string Name { get; set; }
            public string Store { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        private class Order
        {
            public int Number { get; set; }
            public string Store { get; set; }
            public List<string> Items { get; set; }
            public string DeliveryStop { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public decimal Total { get; set; }
        }

        private class DetectedObject
        {
            public string ObjectName { get; set; }
            public string Details { get; set; }
            public string Floor { get; set; }
            public DateTime Time { get; set; }
        }

        private class NotificationItem
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public NotificationPriority Priority { get; set; }
            public DateTime Time { get; set; }
        }

        private enum NotificationPriority
        {
            Normal,
            High
        }
    }
}
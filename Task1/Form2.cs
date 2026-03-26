using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task1
{
    public partial class Form2 : Form
    {
        private readonly string _url = "https://ydstvssozszyririptsi.supabase.co";
        private readonly string _key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inlkc3R2c3NvenN6eXJpcmlwdHNpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzMzNjUxOTQsImV4cCI6MjA4ODk0MTE5NH0.w8ZEsp8CPvvZPowUO92cFDtsPIPhnk3AVwi0ya2lAF4";
        private HttpClient _httpClient;
        private bool _isRegisterMode = false;
        private string _selectedImagePath = null;
        private byte[] _imageBytes = null;
        private readonly string _bucketName = "avatars";

        public Form2()
        {
            InitializeComponent();

            btnLogin.Text = "Войти";
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            btnClear.Click += BtnClear_Click;
            btnSelectAvatar.Click += BtnSelectAvatar_Click;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _key);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _key);

            SetLoginMode();
        }

        // ВЫБОР АВАТАРКИ
        private void BtnSelectAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
                openFileDialog.Title = "Выберите аватарку";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _selectedImagePath = openFileDialog.FileName;

                        long fileSize = new FileInfo(_selectedImagePath).Length;
                        if (fileSize > 5 * 1024 * 1024)
                        {
                            MessageBox.Show("Файл слишком большой! Максимум 5 МБ.",
                                "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            _selectedImagePath = null;
                            return;
                        }

                        Image img = Image.FromFile(_selectedImagePath);

                        if (img.Width > 1024 || img.Height > 1024)
                        {
                            if (MessageBox.Show("Изображение больше 1024x1024. Всё равно использовать?",
                                "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                img.Dispose();
                                _selectedImagePath = null;
                                return;
                            }
                        }

                        pictureBoxAvatar.Image = new Bitmap(img);
                        img.Dispose();

                        _imageBytes = File.ReadAllBytes(_selectedImagePath);

                        label4.Text = "✓ " + Path.GetFileName(_selectedImagePath);
                        label4.ForeColor = Color.DarkGreen;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _selectedImagePath = null;
                        _imageBytes = null;
                    }
                }
            }
        }

        // ЗАГРУЗКА В SUPABASE STORAGE
        private async Task<string> UploadAvatarAsync(string username)
        {
            if (_imageBytes == null || _imageBytes.Length == 0)
                return null;

            try
            {
                string fileExtension = Path.GetExtension(_selectedImagePath).ToLower();

                string safeUsername = new string(username.Where(c => char.IsLetterOrDigit(c) && c < 128).ToArray());
                if (string.IsNullOrEmpty(safeUsername))
                    safeUsername = "user";

                string fileName = safeUsername + "_" + Guid.NewGuid().ToString("N") + fileExtension;

                var uploadUrl = _url + "/storage/v1/object/" + _bucketName + "/" + fileName;

                using (var uploadClient = new HttpClient())
                {
                    uploadClient.DefaultRequestHeaders.Add("apikey", _key);
                    uploadClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _key);
                    uploadClient.DefaultRequestHeaders.Add("x-upsert", "true");

                    string contentType = GetMimeType(fileExtension);

                    var binaryContent = new ByteArrayContent(_imageBytes);
                    binaryContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    var response = await uploadClient.PostAsync(uploadUrl, binaryContent);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return _url + "/storage/v1/object/public/" + _bucketName + "/" + fileName;
                    }
                    else
                    {
                        throw new Exception("Storage error: " + response.StatusCode + " - " + responseContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки аватарки: " + ex.Message);
                return null;
            }
        }

        // ОПРЕДЕЛЕНИЕ MIME ТИПА
        private string GetMimeType(string extension)
        {
            string ext = extension.ToLower();

            if (ext == ".jpg" || ext == ".jpeg")
                return "image/jpeg";
            else if (ext == ".png")
                return "image/png";
            else if (ext == ".gif")
                return "image/gif";
            else if (ext == ".bmp")
                return "image/bmp";
            else if (ext == ".webp")
                return "image/webp";
            else
                return "application/octet-stream";
        }

        // ВХОД 
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (_isRegisterMode)
            {
                SetLoginMode();
                return;
            }

            label3.Text = "";
            label4.Text = "";

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text))
            {
                label3.Text = "Введите логин и пароль";
                label3.ForeColor = Color.DarkRed;
                return;
            }

            try
            {
                label3.Text = "Проверка...";
                label3.ForeColor = Color.Blue;

                string username = textBox1.Text.Trim();
                string password = textBox2.Text;

                var url = _url + "/rest/v1/users?username=eq." + username + "&password=eq." + password;
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    label3.Text = "✗ Ошибка: " + response.StatusCode;
                    label3.ForeColor = Color.DarkRed;
                    return;
                }

                var users = JsonSerializer.Deserialize<JsonElement>(content);

                if (users.GetArrayLength() > 0)
                {
                    label3.Text = "✓ Вход выполнен!";
                    label3.ForeColor = Color.DarkGreen;
                    label4.Text = "Добро пожаловать, " + username + "!";
                    label4.ForeColor = Color.DarkGreen;

                    await Task.Delay(1000);
                    this.Hide();
                    new Form1().Show();
                }
                else
                {
                    label3.Text = "✗ Неверный логин или пароль";
                    label3.ForeColor = Color.DarkRed;
                    label4.Text = "Или такого пользователя не существует";
                    label4.ForeColor = Color.DarkRed;
                }
            }
            catch (Exception ex)
            {
                label3.Text = "✗ Ошибка: " + ex.Message;
                label4.Text = ex.InnerException != null ? ex.InnerException.Message : "Нет деталей";
                label3.ForeColor = Color.DarkRed;
                label4.ForeColor = Color.DarkRed;
            }
        }

        // РЕГИСТРАЦИЯ
        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (!_isRegisterMode)
            {
                SetRegisterMode();
                return;
            }

            label3.Text = "";
            label4.Text = "";

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                label3.Text = "Заполните все поля";
                label3.ForeColor = Color.DarkRed;
                return;
            }

            if (textBox2.Text != textBox3.Text)
            {
                label3.Text = "Пароли не совпадают";
                label3.ForeColor = Color.DarkRed;
                return;
            }

            if (textBox2.Text.Length < 6)
            {
                label3.Text = "Пароль минимум 6 символов";
                label3.ForeColor = Color.DarkRed;
                return;
            }

            try
            {
                string username = textBox1.Text.Trim();
                string password = textBox2.Text;

                label3.Text = "Проверка пользователя...";
                label3.ForeColor = Color.Blue;

                var checkUrl = _url + "/rest/v1/users?username=eq." + username;
                var checkResponse = await _httpClient.GetAsync(checkUrl);
                var checkContent = await checkResponse.Content.ReadAsStringAsync();

                var existingUsers = JsonSerializer.Deserialize<JsonElement>(checkContent);
                if (existingUsers.GetArrayLength() > 0)
                {
                    label3.Text = "Пользователь уже существует";
                    label3.ForeColor = Color.DarkRed;
                    return;
                }

                string avatarUrl = null;
                if (_imageBytes != null && _imageBytes.Length > 0)
                {
                    label3.Text = "Загрузка аватарки...";
                    avatarUrl = await UploadAvatarAsync(username);
                }

                label3.Text = "Создание пользователя...";

                var userData = new
                {
                    username = username,
                    password = password,
                    avatar_url = avatarUrl
                };

                var json = JsonSerializer.Serialize(userData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var insertUrl = _url + "/rest/v1/users";
                var response = await _httpClient.PostAsync(insertUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    label3.Text = "✓ Регистрация успешна!";
                    label3.ForeColor = Color.DarkGreen;
                    label4.Text = "Теперь войдите, " + username + "!";
                    label4.ForeColor = Color.DarkGreen;

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    pictureBoxAvatar.Image = null;
                    _imageBytes = null;
                    _selectedImagePath = null;

                    await Task.Delay(1500);
                    SetLoginMode();
                }
                else
                {
                    label3.Text = "✗ Ошибка: " + response.StatusCode;
                    label4.Text = responseContent;
                    label3.ForeColor = Color.DarkRed;
                    label4.ForeColor = Color.DarkRed;
                }
            }
            catch (Exception ex)
            {
                label3.Text = "✗ Ошибка: " + ex.Message;
                label4.Text = ex.InnerException != null ? ex.InnerException.Message : "Нет деталей";
                label3.ForeColor = Color.DarkRed;
                label4.ForeColor = Color.DarkRed;
            }
        }

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ 
        private void SetLoginMode()
        {
            _isRegisterMode = false;
            btnLogin.Text = "Войти";
            btnRegister.Text = "Регистрация";
            this.Text = "Вход в систему";

            textBox3.Visible = false;
            label2.Text = "Пароль:";

            pictureBoxAvatar.Visible = false;
            btnSelectAvatar.Visible = false;
            labelAvatar.Visible = false;

            label3.Text = "";
            label4.Text = "";
        }

        private void SetRegisterMode()
        {
            _isRegisterMode = true;
            btnLogin.Text = "← Вход";
            btnRegister.Text = "Зарегистрироваться";
            this.Text = "Регистрация";

            textBox3.Visible = true;
            label2.Text = "Пароль:";

            pictureBoxAvatar.Visible = true;
            btnSelectAvatar.Visible = true;
            labelAvatar.Visible = true;

            label3.Text = "";
            label4.Text = "";
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            pictureBoxAvatar.Image = null;
            _imageBytes = null;
            _selectedImagePath = null;
            label3.Text = "";
            label4.Text = "";
            textBox1.Focus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_httpClient != null)
                _httpClient.Dispose();
            base.OnFormClosing(e);
        }
    }
}
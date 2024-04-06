using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Client.Services;

namespace Client;
public partial class MainWindow : Window
{
    private ApiClient _apiClient;
    private JsonSerializerSettings jsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    public MainWindow()
    {
        InitializeComponent();
        _apiClient = new();
    }

    private async void LoginButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        LoginButton.IsEnabled = false;
        UsernameTextBox.IsEnabled = false;
        PasswordBox.IsEnabled = false;
        try
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            InvalidLoginTextBlock.Visibility = Visibility.Collapsed;
            LoggingTextBlock.Visibility = Visibility.Visible;
            var loginOk = await _apiClient.Login(username, password);
            LoggingTextBlock.Visibility = Visibility.Collapsed;
            if (loginOk is null)
            {
                InvalidLoginTextBlock.Visibility = Visibility.Visible;
                return;
            }
            if (loginOk == "Exception")
            {
                MessageBox.Show("The API service is unreachable");
                return;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                InvalidLoginTextBlock.Visibility = Visibility.Visible;
                return;
            }
            InvalidLoginTextBlock.Visibility = Visibility.Collapsed;
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            LoginGrid.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
            MenuViewGrid.Visibility = Visibility.Visible;
            LoginIdTextBlock.Text = $"Logged in as {username}";
        }
        finally
        {
            LoginButton.IsEnabled = true;
            UsernameTextBox.IsEnabled = true;
            PasswordBox.IsEnabled = true;
        }
    }

    private void CollapseAll()
    {
        List<FrameworkElement> ignoreList = [MenuTitleTextBlock, LoginIdTextBlock, LogoutButton];
        foreach (var child in MainMenuGrid.Children)
        {
            if (child is FrameworkElement element && !ignoreList.Contains(element) && element.Visibility != Visibility.Collapsed)
            {
                element.Visibility = Visibility.Collapsed;
            }
        }
    }

    private static void ClearControls(Grid grid)
    {
        foreach (var control in grid.Children)
        {
            if (control is TextBox textBox)
            {
                textBox.Text = "";
            }
            else if (control is DatePicker datePicker)
            {
                datePicker.SelectedDate = null;
            }
            else if (control is PasswordBox passwordBox)
            {
                passwordBox.Password = "";
            }
            else if (control is DataGrid dataGrid)
            {
                dataGrid.ItemsSource = null;
            }
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        _apiClient = new();
        ClearControls(AppointmentsGrid);
        ClearControls(DoctorsGrid);
        ClearControls(StaffGrid);
        ClearControls(PatientsGrid);
        CollapseAll();
        MainMenuGrid.Visibility = Visibility.Collapsed;
        LoginGrid.Visibility = Visibility.Visible;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        CollapseAll();
        MenuViewGrid.Visibility = Visibility.Visible;
    }

    private void AppointmentsButton_Click(object sender, RoutedEventArgs e)
    {
        MenuViewGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Visible;
        AppointmentsGrid.Visibility = Visibility.Visible;
    }

    private void DoctorsButton_Click(object sender, RoutedEventArgs e)
    {
        MenuViewGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Visible;
        DoctorsGrid.Visibility = Visibility.Visible;
    }

    private void StaffButton_Click(object sender, RoutedEventArgs e)
    {
        MenuViewGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Visible;
        StaffGrid.Visibility = Visibility.Visible;
    }

    private void PatientsButton_Click(object sender, RoutedEventArgs e)
    {
        MenuViewGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Visible;
        PatientsGrid.Visibility = Visibility.Visible;
    }

    private void AppointmentBackButton_Click(object sender, RoutedEventArgs e)
    {
        ClearControls(AppointmentFieldsGrid);
        ClearControls(DeliverResultsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        AppointmentsGrid.Visibility = Visibility.Visible;
    }

    private void DoctorBackButton_Click(object sender, RoutedEventArgs e)
    {
        ClearControls(DoctorFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        DoctorsGrid.Visibility = Visibility.Visible;
    }

    private void StaffBackButton_Click(object sender, RoutedEventArgs e)
    {
        ClearControls(StaffFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        StaffGrid.Visibility = Visibility.Visible;
    }

    private void PatientBackButton_Click(object sender, RoutedEventArgs e)
    {
        ClearControls(PatientFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        PatientsGrid.Visibility = Visibility.Visible;
    }

    private async void DeliverTestSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        var results = AppointmentResultsBox.Text;
        var isCompleted = true;
        var resultsObject = new { results, isCompleted };
        var json = JsonConvert.SerializeObject(resultsObject);
        string title = DeliverResultsTitle.Text;
        Match match = Regex.Match(title, @"#(\d+)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value;
            int id = int.Parse(number);
            var patchOk = await _apiClient.Patch("appointments", json, id);
            if (patchOk == "Ok")
            {
                MessageBox.Show($"Results delivered successfully");
                ClearControls(AppointmentFieldsGrid);
                CollapseAll();
                MenuBackButton.Visibility = Visibility.Visible;
                AppointmentsGrid.Visibility = Visibility.Visible;
                return;
            }
            MessageBox.Show($"Error delivering results:\n{patchOk}");
            return;
        }
        MessageBox.Show("Invalid ID Number");
        ClearControls(AppointmentFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        AppointmentsGrid.Visibility = Visibility.Visible;
    }

    private async void NewAppointmentSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PatientIdAppointmentBox.Text, out int patientId))
        {
            patientId = -1;
        }
        if (!int.TryParse(DoctorIdAppointmentBox.Text, out int doctorId))
        {
            doctorId = -1;
        }
        Appointment newAppointment = new()
        {
            PatientId = patientId,
            DoctorId = doctorId,
            Title = AppointmentTitleBox.Text,
            Details = AppointmentDetailsBox.Text,
            AppointmentDate = AppointmentDateBox.SelectedDate ?? DateTime.Now,
        };
        var json = JsonConvert.SerializeObject(newAppointment, jsonSettings);
        var postOk = await _apiClient.Post("appointments", json);
        if (postOk == "Ok")
        {
            MessageBox.Show($"Appointment added successfully");
            ClearControls(AppointmentFieldsGrid);
            CollapseAll();
            MenuBackButton.Visibility = Visibility.Visible;
            AppointmentsGrid.Visibility = Visibility.Visible;
            return;
        }
        MessageBox.Show($"Error adding appointment:\n{postOk}");
    }

    private async void AppointmentDetailsSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PatientIdAppointmentBox.Text, out int patientId))
        {
            patientId = -1;
        }
        if (!int.TryParse(DoctorIdAppointmentBox.Text, out int doctorId))
        {
            doctorId = -1;
        }
        string title = AppointmentDetailsTitle.Text;
        Match match = Regex.Match(title, @"#(\d+)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value;
            int id = int.Parse(number);
            Appointment appointment = new()
            {
                PatientId = patientId < 0 ? null : patientId,
                DoctorId = doctorId < 0 ? null : doctorId,
                Title = AppointmentTitleBox.Text,
                Details = AppointmentDetailsBox.Text,
                AppointmentDate = AppointmentDateBox.SelectedDate ?? null,
            };
            var json = JsonConvert.SerializeObject(appointment, jsonSettings);
            var patchOk = await _apiClient.Patch("appointments", json, id);
            if (patchOk == "Ok")
            {
                MessageBox.Show($"Appointment updated successfully");
                ClearControls(AppointmentFieldsGrid);
                CollapseAll();
                MenuBackButton.Visibility = Visibility.Visible;
                AppointmentsGrid.Visibility = Visibility.Visible;
                return;
            }
            MessageBox.Show($"Error updating appointment:\n{patchOk}");
            return;
        }
        MessageBox.Show("Invalid ID Number");
        ClearControls(AppointmentFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        AppointmentsGrid.Visibility = Visibility.Visible;
    }

    private async void GetAllAppointmentsButton_Click(object sender, RoutedEventArgs e)
    {
        var appointments = await _apiClient.GetAllItems<Appointment>("appointments");
        if (appointments is null)
        {
            return;
        }
        AppointmentsDataGrid.ItemsSource = appointments;
    }

    private async void GetAppointmentButton_Click(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Appointment ID:", "Number Input", "0");
        if (int.TryParse(result, out int id))
        {
            var appointment = await _apiClient.GetItemById<Appointment>("appointments", id);
            if (appointment is null || appointment.Equals(default(Appointment)))
            {
                MessageBox.Show($"Appointment with ID {id} does not exist");
                return;
            }
            AppointmentDetailsTitle.Text = $"Appointment Details - #{id}";
            AppointmentsGrid.Visibility = Visibility.Collapsed;
            MenuBackButton.Visibility = Visibility.Collapsed;
            AppointmentDetailsGrid.Visibility = Visibility.Visible;
            AppointmentFieldsGrid.Visibility = Visibility.Visible;
            AppointmentBackButton.Visibility = Visibility.Visible;
            AppointmentTitleBox.Text = appointment.Title;
            PatientIdAppointmentBox.Text = appointment.PatientId.ToString();
            DoctorIdAppointmentBox.Text = appointment.DoctorId.ToString();
            AppointmentDateBox.SelectedDate = appointment.AppointmentDate;
            AppointmentDetailsBox.Text = appointment.Details;
            return;
        }
        MessageBox.Show("Invalid appointment ID");
    }

    private void AddAppointmentButton_Click(object sender, RoutedEventArgs e)
    {

        AppointmentsGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Collapsed;
        NewAppointmentGrid.Visibility = Visibility.Visible;
        AppointmentFieldsGrid.Visibility = Visibility.Visible;
        AppointmentBackButton.Visibility = Visibility.Visible;
    }

    private async void GetAllDoctorsButton_Click(object sender, RoutedEventArgs e)
    {
        var doctors = await _apiClient.GetAllItems<Doctor>("doctors");
        if (doctors is null)
        {
            return;
        }
        DoctorsDataGrid.ItemsSource = doctors;
    }

    private async void GetDoctorButton_Click(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Doctor ID:", "Number Input", "0");
        if (int.TryParse(result, out int id))
        {
            var doctor = await _apiClient.GetItemById<Doctor>("doctors", id);
            if (doctor is null || doctor.Equals(default(Doctor)))
            {
                MessageBox.Show($"Doctor with ID {id} does not exist");
                return;
            }
            DoctorDetailsTitle.Text = $"Doctor Details - #{id}";
            DoctorsGrid.Visibility = Visibility.Collapsed;
            MenuBackButton.Visibility = Visibility.Collapsed;
            DoctorDetailsGrid.Visibility = Visibility.Visible;
            DoctorFieldsGrid.Visibility = Visibility.Visible;
            DoctorBackButton.Visibility = Visibility.Visible;
            DoctorNameFieldBox.Text = doctor.Name;
            DoctorAgeFieldBox.Text = doctor.Age.ToString();
            DoctorGenderFieldBox.Text = doctor.Gender;
            DoctorAddressFieldBox.Text = doctor.Address;
            DoctorPhoneFieldBox.Text = doctor.Phone;
            DoctorEmailFieldBox.Text = doctor.Email;
            DoctorStatusFieldBox.Text = doctor.Status;
            DoctorUsernameFieldBox.Text = doctor.Username;
            DoctorSpecializationFieldBox.Text = doctor.Specialization;
            return;
        }
        MessageBox.Show("Invalid doctor ID");
    }

    private void AddDoctorButton_Click(object sender, RoutedEventArgs e)
    {
        DoctorsGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Collapsed;
        NewDoctorGrid.Visibility = Visibility.Visible;
        DoctorFieldsGrid.Visibility = Visibility.Visible;
        DoctorBackButton.Visibility = Visibility.Visible;
    }

    private async void NewDoctorSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(DoctorAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        Doctor newDoctor = new()
        {
            Name = DoctorNameFieldBox.Text,
            Age = age,
            Gender = DoctorGenderFieldBox.Text,
            Address = DoctorAddressFieldBox.Text,
            Phone = DoctorPhoneFieldBox.Text,
            Email = DoctorEmailFieldBox.Text,
            Status = DoctorStatusFieldBox.Text,
            Username = DoctorUsernameFieldBox.Text,
            Password = DoctorPasswordFieldBox.Password,
            Specialization = DoctorSpecializationFieldBox.Text
        };
        var json = JsonConvert.SerializeObject(newDoctor, jsonSettings);
        var postOk = await _apiClient.Post("doctors", json);
        if (postOk == "Ok")
        {
            MessageBox.Show($"Doctor added successfully");
            ClearControls(DoctorFieldsGrid);
            CollapseAll();
            MenuBackButton.Visibility = Visibility.Visible;
            DoctorsGrid.Visibility = Visibility.Visible;
            return;
        }
        MessageBox.Show($"Error adding doctor:\n{postOk}");
    }

    private async void DoctorDetailsSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(DoctorAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        string title = DoctorDetailsTitle.Text;
        Match match = Regex.Match(title, @"#(\d+)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value;
            int id = int.Parse(number);
            Doctor doctor = new()
            {
                Name = DoctorNameFieldBox.Text,
                Age = age < 0 ? null : age,
                Gender = DoctorGenderFieldBox.Text,
                Address = DoctorAddressFieldBox.Text,
                Phone = DoctorPhoneFieldBox.Text,
                Email = DoctorEmailFieldBox.Text,
                Status = DoctorStatusFieldBox.Text,
                Username = DoctorUsernameFieldBox.Text,
                Password = DoctorPasswordFieldBox.Password,
                Specialization = DoctorSpecializationFieldBox.Text
            };
            var json = JsonConvert.SerializeObject(doctor, jsonSettings);
            var patchOk = await _apiClient.Patch("doctors", json, id);
            if (patchOk == "Ok")
            {
                MessageBox.Show($"Doctor updated successfully");
                ClearControls(DoctorFieldsGrid);
                CollapseAll();
                MenuBackButton.Visibility = Visibility.Visible;
                DoctorsGrid.Visibility = Visibility.Visible;
                return;
            }
            MessageBox.Show($"Error updating doctor:\n{patchOk}");
            return;
        }
        MessageBox.Show("Invalid ID Number");
        ClearControls(DoctorFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        DoctorsGrid.Visibility = Visibility.Visible;
    }

    private async void GetAllStaffButton_Click(object sender, RoutedEventArgs e)
    {
        var staff = await _apiClient.GetAllItems<StaffMember>("staff");
        if (staff is null)
        {
            return;
        }
        StaffDataGrid.ItemsSource = staff;
    }

    private async void GetStaffButton_Click(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Staff Member ID:", "Number Input", "0");
        if (int.TryParse(result, out int id))
        {            
            var staff = await _apiClient.GetItemById<StaffMember>("staff", id);
            if (staff is null || staff.Equals(default(StaffMember)))
            {
                MessageBox.Show($"Staff Member with ID {id} does not exist");
                return;
            }
            StaffDetailsTitle.Text = $"Staff Member Details - #{id}";
            StaffGrid.Visibility = Visibility.Collapsed;
            MenuBackButton.Visibility = Visibility.Collapsed;
            StaffDetailsGrid.Visibility = Visibility.Visible;
            StaffFieldsGrid.Visibility = Visibility.Visible;
            StaffBackButton.Visibility = Visibility.Visible;
            StaffNameFieldBox.Text = staff.Name;
            StaffAgeFieldBox.Text = staff.Age.ToString();
            StaffGenderFieldBox.Text = staff.Gender;
            StaffAddressFieldBox.Text = staff.Address;
            StaffPhoneFieldBox.Text = staff.Phone;
            StaffEmailFieldBox.Text = staff.Email;
            StaffStatusFieldBox.Text = staff.Status;
            StaffUsernameFieldBox.Text = staff.Username;
            return;
        }
        MessageBox.Show("Invalid staff member ID");
    }

    private async void StaffDetailsSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(StaffAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        string title = StaffDetailsTitle.Text;
        Match match = Regex.Match(title, @"#(\d+)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value;
            int id = int.Parse(number);
            StaffMember staff = new()
            {
                Name = StaffNameFieldBox.Text,
                Age = age < 0 ? null : age,
                Gender = StaffGenderFieldBox.Text,
                Address = StaffAddressFieldBox.Text,
                Phone = StaffPhoneFieldBox.Text,
                Email = StaffEmailFieldBox.Text,
                Status = StaffStatusFieldBox.Text,
                Username = StaffUsernameFieldBox.Text,
                Password = StaffPasswordFieldBox.Password,
            };
            var json = JsonConvert.SerializeObject(staff, jsonSettings);
            var patchOk = await _apiClient.Patch("staff", json, id);
            if (patchOk == "Ok")
            {
                MessageBox.Show($"Staff member updated successfully");
                ClearControls(StaffFieldsGrid);
                CollapseAll();
                MenuBackButton.Visibility = Visibility.Visible;
                StaffGrid.Visibility = Visibility.Visible;
                return;
            }
            MessageBox.Show($"Error updating staff member:\n{patchOk}");
            return;
        }
        MessageBox.Show("Invalid ID Number");
        ClearControls(StaffFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        StaffGrid.Visibility = Visibility.Visible;
    }

    private async void NewStaffSubmitButton_Click(object sender, RoutedEventArgs e)
    {       
        if (!int.TryParse(StaffAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        StaffMember newStaff = new()
        {
            Name = StaffNameFieldBox.Text,
            Age = age,
            Gender = StaffGenderFieldBox.Text,
            Address = StaffAddressFieldBox.Text,
            Phone = StaffPhoneFieldBox.Text,
            Email = StaffEmailFieldBox.Text,
            Status = StaffStatusFieldBox.Text,
            Username = StaffUsernameFieldBox.Text,
            Password = StaffPasswordFieldBox.Password
        };
        var json = JsonConvert.SerializeObject(newStaff, jsonSettings);
        var postOk = await _apiClient.Post("staff", json);
        if (postOk == "Ok")
        {
            MessageBox.Show($"Staff member added successfully");
            ClearControls(StaffFieldsGrid);
            CollapseAll();
            MenuBackButton.Visibility = Visibility.Visible;
            StaffGrid.Visibility = Visibility.Visible;
            return;
        }
        MessageBox.Show($"Error adding staff member:\n{postOk}");
    }

    private void AddStaffButton_Click(object sender, RoutedEventArgs e)
    {
        StaffGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Collapsed;
        NewStaffGrid.Visibility = Visibility.Visible;
        StaffFieldsGrid.Visibility = Visibility.Visible;
        StaffBackButton.Visibility = Visibility.Visible;
    }

    private async void GetAllPatientsButton_Click(object sender, RoutedEventArgs e)
    {
        var patients = await _apiClient.GetAllItems<Patient>("patients");
        if (patients is null)
        {
            return;
        }
        PatientsDataGrid.ItemsSource = patients;
    }

    private async void PatientDetailsSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PatientAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        string title = PatientDetailsTitle.Text;
        Match match = Regex.Match(title, @"#(\d+)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value;
            int id = int.Parse(number);
            Patient patient = new()
            {
                Name = PatientNameFieldBox.Text,
                Age = age < 0 ? null : age,
                Gender = PatientGenderFieldBox.Text,
                Address = PatientAddressFieldBox.Text,
                Phone = PatientPhoneFieldBox.Text,
                Email = PatientEmailFieldBox.Text,
                BirthDate = PatientBirthDateBox.SelectedDate ?? null,
                BloodGroup = PatientBloodGroupFieldBox.Text,
                Allergies = PatientAllergiesFieldBox.Text,
                AdditionalInfo = PatienAdditionalInfoFieldBox.Text,
            };
            var json = JsonConvert.SerializeObject(patient, jsonSettings);
            var patchOk = await _apiClient.Patch("patients", json, id);
            if (patchOk == "Ok")
            {
                MessageBox.Show($"Patient updated successfully");
                ClearControls(PatientFieldsGrid);
                CollapseAll();
                MenuBackButton.Visibility = Visibility.Visible;
                PatientsGrid.Visibility = Visibility.Visible;
                return;
            }
            MessageBox.Show($"Error updating patient:\n{patchOk}");
            return;
        }
        MessageBox.Show("Invalid ID Number");
        ClearControls(PatientFieldsGrid);
        CollapseAll();
        MenuBackButton.Visibility = Visibility.Visible;
        PatientsGrid.Visibility = Visibility.Visible;
    }

    private async void NewPatientSubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PatientAgeFieldBox.Text, out int age))
        {
            age = -1;
        }
        Patient newPatient = new()
        {
            Name = PatientNameFieldBox.Text,
            Age = age,
            Gender = PatientGenderFieldBox.Text,
            Address = PatientAddressFieldBox.Text,
            Phone = PatientPhoneFieldBox.Text,
            Email = PatientEmailFieldBox.Text,
            BirthDate = PatientBirthDateBox.SelectedDate ?? DateTime.Now,
            BloodGroup = PatientBloodGroupFieldBox.Text,
            Allergies = PatientAllergiesFieldBox.Text,
            AdditionalInfo = PatienAdditionalInfoFieldBox.Text
        };
        var json = JsonConvert.SerializeObject(newPatient, jsonSettings);
        var postOk = await _apiClient.Post("patients", json);
        if (postOk == "Ok")
        {
            MessageBox.Show($"Patient added successfully");
            ClearControls(PatientFieldsGrid);
            CollapseAll();
            MenuBackButton.Visibility = Visibility.Visible;
            PatientsGrid.Visibility = Visibility.Visible;
            return;
        }
        MessageBox.Show($"Error adding patient:\n{postOk}");
    }

    private async void GetPatientButton_Click(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Patient ID:", "Number Input", "0");
        if (int.TryParse(result, out int id))
        {
            var patient = await _apiClient.GetItemById<Patient>("patients", id);
            if (patient is null || patient.Equals(default(Patient)))
            {
                MessageBox.Show($"Patient with ID {id} does not exist");
                return;
            }
            PatientDetailsTitle.Text = $"Patient Details - #{id}";
            PatientsGrid.Visibility = Visibility.Collapsed;
            MenuBackButton.Visibility = Visibility.Collapsed;
            PatientDetailsGrid.Visibility = Visibility.Visible;
            PatientFieldsGrid.Visibility = Visibility.Visible;
            PatientBackButton.Visibility = Visibility.Visible;
            PatientNameFieldBox.Text = patient.Name;
            PatientAgeFieldBox.Text = patient.Age.ToString();
            PatientGenderFieldBox.Text = patient.Gender;
            PatientAddressFieldBox.Text = patient.Address;
            PatientPhoneFieldBox.Text = patient.Phone;
            PatientEmailFieldBox.Text = patient.Email;
            PatientBirthDateBox.SelectedDate = patient.BirthDate;
            PatientBloodGroupFieldBox.Text = patient.BloodGroup;
            PatientAllergiesFieldBox.Text = patient.Allergies;
            PatienAdditionalInfoFieldBox.Text = patient.AdditionalInfo;
            return;
        }
        MessageBox.Show("Invalid patient ID");
    }

    private void AddPatientButton_Click(object sender, RoutedEventArgs e)
    {
        PatientsGrid.Visibility = Visibility.Collapsed;
        MenuBackButton.Visibility = Visibility.Collapsed;
        NewPatientGrid.Visibility = Visibility.Visible;
        PatientFieldsGrid.Visibility = Visibility.Visible;
        PatientBackButton.Visibility = Visibility.Visible;
    }

    private async void DeliverResultsButton_Click(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Appointment ID:", "Number Input", "0");
        if (int.TryParse(result, out int id))
        {
            var appointment = await _apiClient.GetItemById<Appointment>("appointments", id);
            if (appointment is null || appointment.Equals(default(Appointment)))
            {
                MessageBox.Show($"Appointment with ID {id} does not exist");
                return;
            }
            AppointmentsGrid.Visibility = Visibility.Collapsed;
            MenuBackButton.Visibility = Visibility.Collapsed;
            DeliverResultsTitle.Text = $"Deliver Results - #{id}";
            DeliverResultsGrid.Visibility = Visibility.Visible;
            AppointmentBackButton.Visibility = Visibility.Visible;
            PatientNameDeliverBox.Text = appointment.PatientName;
            DoctorNameDeliverBox.Text = appointment.DoctorName;
            AppointmentResultsBox.Text = appointment.Results;
            return;
        }
        MessageBox.Show("Invalid appointment ID");
    }
}

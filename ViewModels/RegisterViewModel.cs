using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MauiFormularioValidado.ViewModels;

public class RegisterViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string email;
    private string nombre;
    private string telefono;
    private string password;
    private string confirmarPassword;

    private readonly Dictionary<string, List<string>> errors = new();

    // ----- PROPIEDADES -----

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre
    {
        get => nombre;
        set
        {
            if (nombre == value) return;
            nombre = value;
            OnPropertyChanged(nameof(Nombre));
            ValidateProperty(value, nameof(Nombre));
        }
    }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo inválido.")]
    public string Email
    {
        get => email;
        set
        {
            if (email == value) return;
            email = value;
            OnPropertyChanged(nameof(Email));
            ValidateProperty(value, nameof(Email));
        }
    }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string Telefono
    {
        get => telefono;
        set
        {
            if (telefono == value) return;
            telefono = value;
            OnPropertyChanged(nameof(Telefono));
            ValidateTelefono();
        }
    }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres.")]
    public string Password
    {
        get => password;
        set
        {
            if (password == value) return;
            password = value;
            OnPropertyChanged(nameof(Password));
            ValidateProperty(value, nameof(Password));
            ValidateConfirmarPassword(); // Validar también confirmación
        }
    }

    [Required(ErrorMessage = "Debes confirmar la contraseña.")]
    public string ConfirmarPassword
    {
        get => confirmarPassword;
        set
        {
            if (confirmarPassword == value) return;
            confirmarPassword = value;
            OnPropertyChanged(nameof(ConfirmarPassword));
            ValidateConfirmarPassword();
        }
    }

    // ----- ERRORES INDIVIDUALES -----

    public string NombreError => GetError(nameof(Nombre));
    public string EmailError => GetError(nameof(Email));
    public string TelefonoError => GetError(nameof(Telefono));
    public string PasswordError => GetError(nameof(Password));
    public string ConfirmarPasswordError => GetError(nameof(ConfirmarPassword));

    // ----- MÉTODOS DE VALIDACIÓN -----

    private string GetError(string propertyName)
        => errors.ContainsKey(propertyName) ? errors[propertyName].FirstOrDefault() : string.Empty;

    private void ValidateProperty(object value, string propertyName)
    {
        if (errors.ContainsKey(propertyName))
            errors.Remove(propertyName);

        var results = new List<ValidationResult>();
        var context = new ValidationContext(this) { MemberName = propertyName };

        if (!Validator.TryValidateProperty(value, context, results))
        {
            errors[propertyName] = results.Select(r => r.ErrorMessage).ToList();
        }

        OnErrorsChanged(propertyName);
        OnPropertyChanged($"{propertyName}Error");
    }

    private void ValidateTelefono()
    {
        const string propertyName = nameof(Telefono);
        if (errors.ContainsKey(propertyName))
            errors.Remove(propertyName);

        if (string.IsNullOrWhiteSpace(Telefono))
        {
            errors[propertyName] = new() { "El teléfono es obligatorio." };
        }
        else if (!Regex.IsMatch(Telefono, @"^(809|829|849)\d{7}$"))
        {
            errors[propertyName] = new() { "Formato: 8095551234" };
        }

        OnErrorsChanged(propertyName);
        OnPropertyChanged(nameof(TelefonoError));
    }

    private void ValidateConfirmarPassword()
    {
        const string propertyName = nameof(ConfirmarPassword);
        if (errors.ContainsKey(propertyName))
            errors.Remove(propertyName);

        if (string.IsNullOrWhiteSpace(ConfirmarPassword))
        {
            errors[propertyName] = new() { "Debes confirmar la contraseña." };
        }
        else if (ConfirmarPassword != Password)
        {
            errors[propertyName] = new() { "Las contraseñas no coinciden." };
        }

        OnErrorsChanged(propertyName);
        OnPropertyChanged(nameof(ConfirmarPasswordError));
    }

    // ----- INTERFACES -----

    public bool HasErrors => errors.Any();
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public IEnumerable GetErrors(string propertyName)
        => errors.ContainsKey(propertyName) ? errors[propertyName] : null;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OnErrorsChanged(string propertyName)
        => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}

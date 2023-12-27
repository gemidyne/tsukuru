using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tsukuru.ViewModels;

public abstract class ViewModelBaseWithValidation : ViewModelBase, INotifyDataErrorInfo, IDataErrorInfo
{
    private readonly Dictionary<string, IList<string>> _validationErrors = new();

    public string this[string propertyName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return Error;
            }

            if (_validationErrors.ContainsKey(propertyName))
            {
                return string.Join(Environment.NewLine, _validationErrors[propertyName]);
            }

            return string.Empty;
        }
    }

    public string Error => string.Join(Environment.NewLine, GetAllErrors());

    public bool HasErrors => _validationErrors.Any();

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _validationErrors.SelectMany(kvp => kvp.Value);

        return _validationErrors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<object>();
    }

    private IEnumerable<string> GetAllErrors()
    {
        return _validationErrors.SelectMany(kvp => kvp.Value).Where(e => !string.IsNullOrWhiteSpace(e));
    }

    public void AddValidationError(string propertyName, string errorMessage)
    {
        if (!_validationErrors.ContainsKey(propertyName))
            _validationErrors.Add(propertyName, new List<string>());

        _validationErrors[propertyName].Add(errorMessage);

        OnPropertyChanged(nameof(Error));
        OnPropertyChanged(nameof(HasErrors));
    }

    public void ClearValidationErrors(string propertyName)
    {
        if (_validationErrors.ContainsKey(propertyName))
        {
            _validationErrors.Remove(propertyName);
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(HasErrors));
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
}
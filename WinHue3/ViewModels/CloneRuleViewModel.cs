using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class CloneRuleViewModel : ValidatableBindableBase
    {
        private ObservableCollection<HueObject> _listRuleObject;
        private ObservableCollection<HueObject> _listReplacementsObject;
        private HueObject _selectedRuleObject;
        private HueObject _selectedReplacementObject;
        private ObservableCollection<CloneRuleModel> _ruleModificationList;
        private CloneRuleModel _selectedReplacement;

        public CloneRuleViewModel()
        {
            _listRuleObject = new ObservableCollection<HueObject>();
            _listReplacementsObject = new ObservableCollection<HueObject>();
            _ruleModificationList = new ObservableCollection<CloneRuleModel>();
        }

        public void Initialize(List<HueObject> listRuleobject)
        {
            ListRuleObject = new ObservableCollection<HueObject>(listRuleobject);
        }

        public ObservableCollection<HueObject> ListReplacementsObject
        {
            get { return _listReplacementsObject; }
            set { SetProperty(ref _listReplacementsObject,value); }
        }

        public ObservableCollection<HueObject> ListRuleObject
        {
            get { return _listRuleObject; }
            set { _listRuleObject = value; }
        }

        public HueObject SelectedRuleObject
        {
            get { return _selectedRuleObject; }
            set { SetProperty(ref _selectedRuleObject, value); }
        }

        public HueObject SelectedReplacementObject
        {
            get { return _selectedReplacementObject; }
            set { SetProperty(ref _selectedReplacementObject, value); }
        }

        private void AddToList()
        {
            ListReplacementsObject.Remove(SelectedRuleObject);
            RuleModificationList.Add(new CloneRuleModel() { replacingobject = SelectedReplacementObject, withobject = SelectedRuleObject });
            SelectedRuleObject = null;
        }

        private void RemoveFromList()
        {
            ListReplacementsObject.Add(SelectedReplacement.replacingobject);
            RuleModificationList.Remove(SelectedReplacement);
            SelectedReplacement = null;
        }

        public ICommand AddToListCommand => new RelayCommand(param => AddToList());

        public ICommand RemoveFromListCommand => new RelayCommand(param => RemoveFromList(), (param) => CanRemove());

        private bool CanRemove()
        {
            return SelectedRuleObject != null;
        }

        public ObservableCollection<CloneRuleModel> RuleModificationList
        {
            get { return _ruleModificationList; }
            set { SetProperty(ref _ruleModificationList,value); }
        }

        public CloneRuleModel SelectedReplacement
        {
            get { return _selectedReplacement; }
            set { SetProperty(ref _selectedReplacement,value); }
        }
    }
}

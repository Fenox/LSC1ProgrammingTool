using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LSC1DatabaseLibrary;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Viewmodel representing a table (e.g. tpos), that defines common methods
    /// but does not contain the type of the data stored, to be more of an abstraction.
    /// </summary>
    public abstract class LSC1TablePropertiesViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Specifies the kind of data that is stored in this table e.g. tpos
        /// </summary>
        public abstract TablesEnum Table { get; }
        /// <summary>
        /// Specifies whether this table needs a step column indicating that the data
        /// in this table is has an order.
        /// </summary>
        public abstract bool HasStep { get; }
        /// <summary>
        /// Specifies whether this table has a name column, this column is a reference
        /// to another item that has a name e.g. a laser proc.
        /// </summary>
        public abstract bool HasNameColumn { get; }
        /// <summary>
        /// specifies whether the data in this table can be filtered by name.
        /// </summary>
        public abstract bool UsesNameFilter { get; }
        public abstract string DataGridName { get; }
        public abstract DataTable DataTable { get; set; }
        public ObservableCollection<string> NameFilterItems { get; set; } = new ObservableCollection<string>();

        public virtual void UpdateNameFilter(string jobId)
        {
        }
    }
}
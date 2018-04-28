namespace LSC1DatabaseEditor.Common.Messages
{
    public class SelectedTreeViewItemChanged
    {
        public object Sender { get; set; }
        public object SelectedItem { get; set; }

        public SelectedTreeViewItemChanged(object sender, object selectedItem)
        {
            Sender = sender;
            SelectedItem = selectedItem;
        }
    }
}

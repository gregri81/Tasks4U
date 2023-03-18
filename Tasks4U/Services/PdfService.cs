using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Tasks4U.Services
{
    public interface IPdfService
    {
        public bool SaveAsPdf(FlowDocument document, IMessageBoxService messageBoxService);
    }

    public class PdfService: IPdfService
    {
        public bool SaveAsPdf(FlowDocument document, IMessageBoxService messageBoxService)
        {
            var printDlg = new PrintDialog();

            document.PageWidth = printDlg.PrintableAreaWidth;
            document.Name = "Tasks_List";

            var printDialog = new PrintDialog();

            var printQueue = new LocalPrintServer().GetPrintQueues().FirstOrDefault(q => q.Name.Contains("PDF"));

            if (printQueue == null)
            {
                messageBoxService.Show("PDF printer not found", "Cannot Save as PDF", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            printDialog.PrintQueue = printQueue;

            printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Document");

            return true;
        }
    }
}

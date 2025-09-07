using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;

namespace TurističkaOrganizacija.GUI
{
    public class UiKitForm1 : UiKit
    {
        //Live search
        public static void WireLiveSearch(IEnumerable<TextBox> boxes, DateTimePicker datePicker, Action refresh, int debounceMs = 250)
        {
            if (boxes == null || refresh == null) return;

            var timer = new Timer { Interval = debounceMs };
            timer.Tick += (s, e) => { timer.Stop(); refresh(); };

            EventHandler onTextChanged = (s, e) => { timer.Stop(); timer.Start(); };
            KeyEventHandler onKeyDown = (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    timer.Stop();
                    refresh();
                }
            };

            foreach (var tb in boxes)
            {
                if (tb == null) continue;
                tb.TextChanged += onTextChanged;
                tb.KeyDown += onKeyDown;
            }

            if (datePicker != null)
            {
                datePicker.ValueChanged += (s, e) => { timer.Stop(); timer.Start(); };
                datePicker.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        timer.Stop();
                        refresh();
                    }
                };
            }
        }

        public static void ClientsLiveSearch(TextBox txtIme, TextBox txtPrezime, TextBox txtPasos,
            TextBox txtEmail, TextBox txtTelefon,
            DateTimePicker dtpRodjenje, Action refresh, int debounceMs = 250)
        {
            WireLiveSearch(new[] { txtIme, txtPrezime, txtPasos, txtEmail, txtTelefon }, dtpRodjenje, refresh, debounceMs);
        }
        //=========================================================================================================
        //designing the clients forum
        public static void ClientsDesign(TextBox txtIme, TextBox txtPrezime, TextBox txtEmail,
            TextBox txtTelefon, TextBox txtPasos, Action RefreshClients, DateTimePicker dtpRodjenje
            )
        {
            TextBoxPlaceholder.SetPlaceholder(txtIme, "Name");
            TextBoxPlaceholder.SetPlaceholder(txtPrezime, "Last Name");
            TextBoxPlaceholder.SetPlaceholder(txtEmail, "Email");
            TextBoxPlaceholder.SetPlaceholder(txtTelefon, "Phone number");
            TextBoxPlaceholder.SetPlaceholder(txtPasos, "Passport");
            UiKitForm1.ClientsLiveSearch(txtIme, txtPrezime, txtPasos, txtEmail, txtTelefon, dtpRodjenje, RefreshClients, 200);
        }
        //=========================================================================================================
        //better clients, change button color
        public static void RefreshClientsAddon(System.Windows.Forms.BindingSource clientsBinding,
            System.Collections.Generic.IEnumerable<TurističkaOrganizacija.Domain.Client> clients,
            TextBox txtEmail, TextBox txtTelefon,
            DateTime rodjenjeBefore, DataGridView gridClients)
        {

            var emailLike = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
            var phoneLike = string.IsNullOrWhiteSpace(txtTelefon.Text) ? null : txtTelefon.Text.Trim();

            var query = (clients ?? System.Linq.Enumerable.Empty<TurističkaOrganizacija.Domain.Client>())
                               .Where(c => c != null);

            if (!string.IsNullOrEmpty(emailLike))
                query = query.Where(c => !string.IsNullOrEmpty(c.Email) &&
                                         c.Email.IndexOf(emailLike, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrEmpty(phoneLike))
                query = query.Where(c => !string.IsNullOrEmpty(c.Phone) &&
                                         c.Phone.Contains(phoneLike));

            // Datum rođenja STROGO pre izabranog datuma; za uključivo promeni < u <=
            query = query.Where(c => c.DateOfBirth < rodjenjeBefore);

            var list = query.ToList();
            clientsBinding.DataSource = null;
            clientsBinding.DataSource = list;
        }

    }
}

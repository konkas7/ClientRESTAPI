using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F1CircuitClient
{
    public partial class Form1 : Form
    {
        private const string BaseUrl = "http://localhost/path/to/php_script.php/";

        public Form1()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await RefreshCircuits();
        }

        private async Task RefreshCircuits()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(BaseUrl + "circuits");
                    response.EnsureSuccessStatusCode(); 

                    string responseBody = await response.Content.ReadAsStringAsync();

                    List<Circuit> circuits = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Circuit>>(responseBody);


                    listViewCircuits.Items.Clear();
                    foreach (Circuit circuit in circuits)
                    {
                        ListViewItem item = new ListViewItem(circuit.CircuitId.ToString());
                        item.SubItems.Add(circuit.Name);
                        listViewCircuits.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtCircuitName.Text;

                using (HttpClient client = new HttpClient())
                {
                    var data = new Dictionary<string, string>
                    {
                        { "name", name }
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(BaseUrl + "circuits/add", content);
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Circuit added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RefreshCircuits();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (listViewCircuits.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a circuit to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int circuitId = int.Parse(listViewCircuits.SelectedItems[0].Text);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync(BaseUrl + $"circuits/delete/{circuitId}");
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Circuit deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RefreshCircuits();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class Circuit
    {
        public int CircuitId { get; set; }
        public string Name { get; set; }
    }
}

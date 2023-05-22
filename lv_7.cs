using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Baze_LV7_predlozak
{


    public partial class Form1 : Form
    {

        //Ovdje deklariraj SQL komande koristeci SQLCommand parametarsku sintaksu za zadatke 1a, 1b, 2 i 3

        private static string SQLInsert = "INSERT INTO osobe (ime, prezime, OIB, datum_rodjenja, spol, visina, broj_cipela) VALUES (@ime, @prezime, @OIB, @datum_rodjenja, @spol, @visina, @broj_cipela)";
        
        private static string SQLUpdate = "UPDATE osobe SET ime = @ime, prezime = @prezime, OIB = @OIB, datum_rodjenja = @datum_rodjenja, spol = @spol, visina = @visina, broj_cipela = @broj_cipela WHERE OIB = @original_OIB";
        
        private static string SQLDelete = "DELETE FROM osobe WHERE OIB = @OIB";
        
        private static string SQLSelect = "SELECT * FROM osobe ORDER BY prezime";


        public Form1()
        {
            InitializeComponent();
            btnDelete.Enabled = false;
        }
        private DBStudent Dbs;

        private void btnSve_Click(object sender, EventArgs e)
        {
            // NE MIJENJAJ

            //Funkcija koja traži od korisnika da unese zaporku
            if (Dbs == null)
            {
                using (FormLogin wl = new FormLogin())  //otvara login prozor
                {
                    wl.ShowDialog();
                    wl.Focus();
                    Dbs = new DBStudent(wl.Pwd);        //kreira klasu za sigurno
                                                        //korištenje zaporke
                    if (string.IsNullOrWhiteSpace(wl.Pwd))
                        return;
                }
            }

            using (SqlConnection conn = Dbs.GetConnection())
            {
                // Kodiraj 1a zadatak u funkciju LOadOsobe
                LoadOsobe(conn);

                if (dgvPodaci.Rows.Count > 0)
                    dgvPodaci.Rows[0].Selected = false;
            }

        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            if (Dbs == null)
                return;

           

            using (SqlConnection conn = Dbs.GetConnection())
            {
                // OVDJE PIŠETE KOD ZA ZADATAK 1. b) i ZADATAK 2.:

                if (dgvPodaci.SelectedRows.Count > 0)
                {
                    // Zadatak 2: Ažuriranje odabrane osobe
                    string originalOIB = dgvPodaci.SelectedRows[0].Cells[0].Value.ToString();

                    using (SqlCommand cmd = new SqlCommand(SQLUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@ime", txtIme.Text);
                        cmd.Parameters.AddWithValue("@prezime", txtPrezime.Text);
                        cmd.Parameters.AddWithValue("@OIB", txtOIB.Text);
                        cmd.Parameters.AddWithValue("@datum_rodjenja", txtDatum.Text);
                        cmd.Parameters.AddWithValue("@spol", rbM.Checked ? "M" : "Ž");
                        cmd.Parameters.AddWithValue("@visina", txtVisina.Text);
                        cmd.Parameters.AddWithValue("@broj_cipela", txtBrCip.Text);
                        cmd.Parameters.AddWithValue("@original_OIB", originalOIB);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Zadatak 1b: Spremanje nove osobe
                    using (SqlCommand cmd = new SqlCommand(SQLInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@ime", txtIme.Text);
                        cmd.Parameters.AddWithValue("@prezime", txtPrezime.Text);
                        cmd.Parameters.AddWithValue("@OIB", txtOIB.Text);
                        cmd.Parameters.AddWithValue("@datum_rodjenja", txtDatum.Text);
                        cmd.Parameters.AddWithValue("@spol", rbM.Checked ? "M" : "Ž");
                        cmd.Parameters.AddWithValue("@visina", txtVisina.Text);
                        cmd.Parameters.AddWithValue("@broj_cipela", txtBrCip.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                // NE MIJENJAJ ispod ove linije ******************
                LoadOsobe(conn);
                SelectCurrentRow();

            }
        }

        //izaberi aktivan red ako ga ima
        private void SelectCurrentRow()
        {
            // NE MIJENJAJ

            int selectedIndex = -1;

            dgvPodaci.ClearSelection();
            if (string.IsNullOrEmpty(txtOIB.Text) && dgvPodaci.Rows.Count > 0)
                selectedIndex = 0;
            else
            {
                foreach (DataGridViewRow row in dgvPodaci.Rows)
                {
                    if (row.Cells[0].Value.ToString().Trim().Equals(txtOIB.Text.Trim()))
                    {
                        selectedIndex = row.Index;
                        break;
                    }
                }
            }
            if (selectedIndex > -1)
            {
                dgvPodaci.Rows[selectedIndex].Selected = true;
                txtOIB.ReadOnly = true;
                btnDelete.Enabled = true;

            }
        }

        public void obrisiSve()
        {
            txtOIB.Text = "";
            txtIme.Text = "";
            txtPrezime.Text = "";
            txtDatum.Text = "";
            txtBrCip.Text = "";
            txtVisina.Text = "";
            dgvPodaci.ClearSelection();
            txtOIB.ReadOnly = false;
            btnDelete.Enabled = false;
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            obrisiSve();
        }

        private void dgvPodaci_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //OVDJE JE DODATAK POTREBAN ZA 2. ZADATAK
            txtIme.Text = dgvPodaci.SelectedRows[0].Cells[1].Value.ToString();
            txtPrezime.Text = dgvPodaci.SelectedRows[0].Cells[2].Value.ToString();
            txtOIB.Text = dgvPodaci.SelectedRows[0].Cells[0].Value.ToString();
            txtDatum.Text = dgvPodaci.SelectedRows[0].Cells[4].Value.ToString();
            if (dgvPodaci.SelectedRows[0].Cells[3].Value.ToString() == "M") 
                rbM.Checked = true;
            else 
                rbZ.Checked = true;
            txtVisina.Text = dgvPodaci.SelectedRows[0].Cells[5].Value.ToString();
            txtBrCip.Text = dgvPodaci.SelectedRows[0].Cells[6].Value.ToString();
            txtOIB.ReadOnly = true;
            btnDelete.Enabled = true;
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = Dbs.GetConnection())
            {
                // OVDJE PIŠETE KOD ZA 3. ZADATAK:
                if (dgvPodaci.SelectedRows.Count > 0)
                {
                    string oib = dgvPodaci.SelectedRows[0].Cells[0].Value.ToString();
                    using (SqlCommand cmd = new SqlCommand(SQLDelete, conn))
                    {
                        cmd.Parameters.AddWithValue("@OIB", oib);
                        cmd.ExecuteNonQuery();
                    }
                }

            // NE MIJENJAJ ispod ove linije
            LoadOsobe(conn);
                dgvPodaci.Rows[0].Selected = false;

            }
            btnDelete.Enabled = false;
            obrisiSve(); 

        }

        private void LoadOsobe(SqlConnection conn)
        {
            // 1a - OVDJE KORISTITE DATA ADAPTER 
            //      Koristite SQLSelect komandu dekariranu na početku ove datoteke
            using (SqlCommand cmd = new SqlCommand(SQLSelect, conn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPodaci.DataSource = dt;
                }
            }

        }

    }


}

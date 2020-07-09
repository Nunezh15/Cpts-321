//-----------------------------------------------------------------------
// <copyright file="Form1.cs" company="Hernan Nunez-Ortega">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------


namespace SpreadSheet_Hernan_Nunez
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        private SpreadsheetEngine.Spreadsheet spreadSheet = new SpreadsheetEngine.Spreadsheet(50, 26);
        public SpreadsheetEngine.UndoRedo undo_redo = new SpreadsheetEngine.UndoRedo();

        public Form1()
        {
            InitializeComponent();

            spreadSheet.CellPropertyChanged += OnCellPropertyChanged;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;

            //UpdateMenuText();

        }

        public void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            //SpreadsheetEngine.Spreadsheet spreadSheet = new SpreadsheetEngine.Spreadsheet(50, 26);


            char i = 'A';
            int iRow = 0;

            //// Columns A to Z
            while (i <= 'Z')
            {
                dataGridView1.Columns.Add("" + i, "" + i);
                i++;
            }

            //// Rows 1 to 50
            while (iRow < 50)
            {
                var tRow = new DataGridViewRow();
                tRow.HeaderCell.Value = (iRow + 1).ToString();
                dataGridView1.Rows.Add(tRow);
                iRow++;
            }

            dataGridView1.RowHeadersWidth = 50;
            //UpdateMenuText();
            //Demo(spreadSheet);
        }


        /// <summary>
        /// Implemented for HW7. This class 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int row = e.RowIndex;
            int column = e.ColumnIndex;

            SpreadsheetEngine.SpreadsheetCell ssCell = spreadSheet.GetCell(row, column);

            dataGridView1.Rows[row].Cells[column].Value = ssCell.Text;
        }


        /// <summary>
        /// Implemented for HW7. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int column = e.ColumnIndex;
            string tText;
            SpreadsheetEngine.IUndoRedoCmd[] undos = new SpreadsheetEngine.IUndoRedoCmd[1];

            SpreadsheetEngine.SpreadsheetCell ssCell = spreadSheet.GetCell(row, column);

            try
            {
                tText = dataGridView1.Rows[row].Cells[column].Value.ToString();
            }
            catch (NullReferenceException)
            {
                tText = "";
            }

            undos[0] = new SpreadsheetEngine.RestoreText(ssCell.Text, ssCell.Name);

            ssCell.Text = tText;

            undo_redo.AddUndo(new SpreadsheetEngine.UndoRedoCollection(undos, "cell text change"));

            dataGridView1.Rows[row].Cells[column].Value = ssCell.Value;

            UpdateMenuText();
        }


        /// <summary>
        /// Demo that will assign text to random cells
        /// </summary>
        /// <param name="spreadsheet"></param>
        //private void demoToolStripMenuItem_Click(object sender, EventArgs e)
        private void Demo(SpreadsheetEngine.Spreadsheet spreadsheet)
        {

            Random rand = new Random();

            //for (int i = 0; i < 50; i++)
            //{
            //    int row = rand.Next(0, 49);
            //    int column = rand.Next(2, 25);

            //    spreadSheet.sheet[row, column].Text = "test";
            //}
            //for (int i = 0; i < 50; i++)
            //{
            //    spreadSheet.sheet[i, 1].Text = "This is cell B" + (i + 1).ToString();
            //}

            //for (int i = 0; i < 50; i++)
            //{
            //    spreadSheet.sheet[i, 0].Text = "=B" + (i + 1).ToString();
            //}


            for (int i = 0; i < 50; i++)
            {
                SpreadsheetEngine.SpreadsheetCell cell = spreadsheet.GetCell(rand.Next(0, 50), rand.Next(0, 26));
                cell.Text = "test";
            }

            for (int i = 0; i < 50; i++)
            {
                SpreadsheetEngine.SpreadsheetCell cell = spreadsheet.GetCell(i, 1);
                cell.Text = (i + 1).ToString();
            }

            for (int i = 0; i < 26; i++)
            {
                SpreadsheetEngine.SpreadsheetCell cell = spreadsheet.GetCell(i, 0);
                cell.Text = "=B" + (i + 1).ToString();
            }
        }

        /// <summary>
        /// will handle the text change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SpreadsheetEngine.SpreadsheetCell backendCell = sender as SpreadsheetEngine.SpreadsheetCell;

            // if the value is changed and it is not null
            if (e.PropertyName == "Value" && backendCell != null)
            {
                dataGridView1.Rows[backendCell.r_index].Cells[backendCell.c_index].Value = backendCell.Value;
            }

            if (e.PropertyName == "BG_Color" && backendCell != null)
            {
                dataGridView1.Rows[backendCell.r_index].Cells[backendCell.c_index].Style.BackColor = System.Drawing.Color.FromArgb(backendCell.BG_Color);
            }
        }





        /// <summary>
        /// Updates the text that says what it is that we are undoing or redoing.
        /// </summary>
        private void UpdateMenuText()
        {
            ToolStripMenuItem menuItems = menuStrip1.Items[0] as ToolStripMenuItem;
            
            foreach (ToolStripItem item in menuItems.DropDownItems)
            {
                if (item.Text.Substring(0, 4) == "Undo")
                {
                    item.Enabled = undo_redo.canUndo; //There was an error here because it was canRedo. 
                    item.Text = "Undo " + undo_redo.undoOp;
                    
                }
                
                else if (item.Text.Substring(0, 4) == "Redo")
                {
                    item.Enabled = undo_redo.canRedo;
                    item.Text = "Redo " + undo_redo.redoOp;
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Menu item for undoing an action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo_redo.Undo(spreadSheet); //Perform the undo.
            UpdateMenuText(); // Will update the text that says what it is that we will undo.
        }

        /// <summary>
        /// Menu item for redoing an action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo_redo.Redo(spreadSheet);
            UpdateMenuText();
        }

        /// <summary>
        /// Clears the spreadsheet. Will be used in the loading function so that we dont load something over the current stuff.
        /// </summary>
        public void Clear()
        {
            int rCount = spreadSheet.RowCount;
            int cCount = spreadSheet.ColumnCount;

            for (int i = 0; i < rCount; i++)
            {
                for (int j = 0; j < cCount; j++)
                {
                    // if the cell is not empty we will clear it out.
                    if (spreadSheet.sheet[i,j].Text != "" || spreadSheet.sheet[i, j].Value != "" || spreadSheet.sheet[i, j].BG_Color != -1)
                    {
                        spreadSheet.sheet[i, j].Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Option to change the background color of a cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cColor = 0;
            ColorDialog colorDialog = new ColorDialog();
            List<SpreadsheetEngine.IUndoRedoCmd> undos = new List<SpreadsheetEngine.IUndoRedoCmd>();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                cColor = colorDialog.Color.ToArgb();

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    SpreadsheetEngine.SpreadsheetCell ss_Cell = spreadSheet.GetCell(cell.RowIndex, cell.ColumnIndex);
                    undos.Add(new SpreadsheetEngine.RestoreBackColor(ss_Cell.BG_Color, ss_Cell.Name));
                    ss_Cell.BG_Color = cColor;
                }

                undo_redo.AddUndo(new SpreadsheetEngine.UndoRedoCollection(undos, "Cell background color change"));
                UpdateMenuText();
            }
        }

        /// <summary>
        /// Allows user to load things from a file into the spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var OFDialog = new OpenFileDialog();
            
            if (OFDialog.ShowDialog() == DialogResult.OK)
            {
                Clear(); //Clear the spreadsheet before we load.

                Stream infileStream = new FileStream(OFDialog.FileName, FileMode.Open, FileAccess.Read);
                spreadSheet.Load(infileStream);

                infileStream.Dispose();

                undo_redo.Clear(); //Clear whatever was on the undo and redo stacks
            }
            UpdateMenuText();
        }


        /// <summary>
        /// Allows user to save work done in the spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var SFDialog = new SaveFileDialog();

            if (SFDialog.ShowDialog() == DialogResult.OK)
            {
                Stream outFileStream = new FileStream(SFDialog.FileName, FileMode.Create, FileAccess.Write);
                spreadSheet.Save(outFileStream);

                outFileStream.Dispose();
            }
        }


    }
}

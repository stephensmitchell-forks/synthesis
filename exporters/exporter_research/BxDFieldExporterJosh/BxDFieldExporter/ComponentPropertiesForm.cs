﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventor;

namespace BxDFieldExporter
{
    // form that allows the user to enter properties for the field types
    public partial class ComponentPropertiesForm : Form
    {
        FieldDataType field;
        public ComponentPropertiesForm()
        {
            InitializeComponent();// inits and populates the form
        }
        // these methods react to changes in the fields so we can save the data
        private void colliderTypeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {// changes the collider box based on the selection
            Type selectedType = null;// make space for a form to add to the form

            switch (colliderTypeCombobox.SelectedIndex)
            {// change selected form based on selection
                case 0: // Box
                    field.colliderType = ColliderType.Box;
                    selectedType = typeof(BoxColliderPropertiesForm);// sets the type to the correct form
                    break;
                case 1: // Sphere
                    field.colliderType = ColliderType.Sphere;
                    selectedType = typeof(SphereColliderPropertiesForm);
                    break;
                case 2: // Mesh
                    field.colliderType = ColliderType.Mesh;
                    selectedType = typeof(MeshColliderPropertiesForm);
                    break;
            }

            if (meshPropertiesTable.Controls.Count > 1)
            {
                if (selectedType == null || meshPropertiesTable.Controls[1].GetType().Equals(selectedType))
                    return;// clears the form so we don't get multiple forms

                meshPropertiesTable.Controls.RemoveAt(1);
            }
            UserControl controller = ((UserControl)Activator.CreateInstance(selectedType));
            meshPropertiesTable.Controls.Add(controller, 0, 1);
            if(field.colliderType == ColliderType.Sphere)// read the data from the file into the form
            {
                ((SphereColliderPropertiesForm)controller).readFromData(field);
            } else if(field.colliderType == ColliderType.Mesh)
            {
                ((MeshColliderPropertiesForm)controller).readFromData(field);
            } else
            {
                ((BoxColliderPropertiesForm)controller).readFromData(field);
            }
        }
        public void MassChanged(object sender, EventArgs e)
        {
            field.Mass = (double) massNumericUpDown.Value;
        }
        private void UpdateFrictionLabel()
        {
            frictionLabel.Text = "Friction:\n" + frictionTrackBar.Value + "/100";
            field.Friction = (double) frictionTrackBar.Value;
        }
        private void frictionTrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateFrictionLabel();
        }
        private void dynamicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamicCheckBox.Checked)
            {
                dynamicGroupBox.Enabled = true;
                field.Dynamic = true;
            }
            else
            {
                dynamicGroupBox.Enabled = false;
                massNumericUpDown.Value = 0;
                field.Dynamic = false;
            }
        }
        public void readFromData(FieldDataType d)
        {// reads from the data so user can see the same values from the last time they entered them
            try
            {
                field = d;
                if (field.colliderType == ColliderType.Sphere)
                {
                    colliderTypeCombobox.SelectedIndex = 1;
                }
                else if (field.colliderType == ColliderType.Mesh)
                {
                    colliderTypeCombobox.SelectedIndex = 2;
                }
                else
                {
                    colliderTypeCombobox.SelectedIndex = 0;
                }
                if (field.Dynamic)
                {
                    dynamicGroupBox.Enabled = true;
                }
                else
                {
                    dynamicGroupBox.Enabled = false;
                    massNumericUpDown.Value = 0;
                }
                frictionTrackBar.Value = (int)field.Friction;
                massNumericUpDown.Value = (decimal)field.Mass;
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class ChangePasswordForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblOld = new System.Windows.Forms.Label();
            this.txtOldPassword = new System.Windows.Forms.TextBox();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = System.Drawing.Color.FromArgb(30, 42, 58);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Смена пароля";
            this.ClientSize = new System.Drawing.Size(320, 190);

            // Labels
            this.lblOld.AutoSize = true;
            this.lblOld.ForeColor = System.Drawing.Color.White;
            this.lblOld.Location = new System.Drawing.Point(20, 20);
            this.lblOld.Text = "Текущий пароль:";

            this.lblNew.AutoSize = true;
            this.lblNew.ForeColor = System.Drawing.Color.White;
            this.lblNew.Location = new System.Drawing.Point(20, 55);
            this.lblNew.Text = "Новый пароль:";

            this.lblConfirm.AutoSize = true;
            this.lblConfirm.ForeColor = System.Drawing.Color.White;
            this.lblConfirm.Location = new System.Drawing.Point(20, 90);
            this.lblConfirm.Text = "Подтверждение:";

            // TextBoxes
            this.txtOldPassword.Location = new System.Drawing.Point(140, 17);
            this.txtOldPassword.Size = new System.Drawing.Size(150, 20);
            this.txtOldPassword.PasswordChar = '*';
            this.txtOldPassword.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.txtOldPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtNewPassword.Location = new System.Drawing.Point(140, 52);
            this.txtNewPassword.Size = new System.Drawing.Size(150, 20);
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtConfirmPassword.Location = new System.Drawing.Point(140, 87);
            this.txtConfirmPassword.Size = new System.Drawing.Size(150, 20);
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Buttons
            this.btnChange.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnChange.FlatStyle = FlatStyle.Flat;
            this.btnChange.ForeColor = System.Drawing.Color.White;
            this.btnChange.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChange.Location = new System.Drawing.Point(60, 130);
            this.btnChange.Size = new System.Drawing.Size(90, 30);
            this.btnChange.Text = "Изменить";
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);

            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(170, 130);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.txtOldPassword);
            this.Controls.Add(this.lblOld);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblOld;
        private System.Windows.Forms.TextBox txtOldPassword;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnCancel;
    }
}
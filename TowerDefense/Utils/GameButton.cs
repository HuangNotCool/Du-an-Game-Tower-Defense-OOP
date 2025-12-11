using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TowerDefense.Utils
{
    public class GameButton : Button
    {
        // --- CẤU HÌNH MÀU SẮC ---
        public Color Color1 { get; set; } = Color.RoyalBlue;
        public Color Color2 { get; set; } = Color.MidnightBlue;
        public Color HoverColor1 { get; set; } = Color.DodgerBlue;
        public Color HoverColor2 { get; set; } = Color.Blue;
        public int BorderRadius { get; set; } = 20;

        // Trạng thái chuột
        private bool _isHovering = false;
        private bool _isPressed = false;

        public GameButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 50);
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;

            // Chống nháy hình khi vẽ lại
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.UserPaint, true);
        }

        // =========================================================
        // HÀM VẼ CHÍNH (QUAN TRỌNG NHẤT)
        // =========================================================
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic; // Vẽ ảnh sắc nét

            Rectangle rect = this.ClientRectangle;
            rect.Width--; rect.Height--; // Giảm 1px để viền mượt

            // 1. VẼ HÌNH DÁNG NÚT (BO TRÒN)
            using (GraphicsPath path = GetRoundedPath(rect, BorderRadius))
            {
                this.Region = new Region(path); // Cắt khung nút

                // 2. TÔ MÀU NỀN (GRADIENT)
                Color c1 = _isHovering ? HoverColor1 : Color1;
                Color c2 = _isHovering ? HoverColor2 : Color2;
                if (_isPressed) { c1 = Color2; c2 = Color1; } // Đảo màu khi nhấn

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, c1, c2, 90F))
                {
                    g.FillPath(brush, path);
                }

                // 3. VẼ VIỀN SÁNG (HIGHLIGHT)
                using (Pen pen = new Pen(Color.FromArgb(80, 255, 255, 255), 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            // =========================================================
            // 4. VẼ NỘI DUNG (ẢNH & CHỮ)
            // =========================================================



            if (this.Image != null)
            {
                // --- TRƯỜNG HỢP CÓ ẢNH (NÚT CHỌN THÁP) ---

                // Tính toán vị trí để vẽ ảnh nằm ở nửa trên
                // Giữ nguyên tỉ lệ hoặc vẽ theo kích thước ảnh đã resize
                int imgW = this.Image.Width;
                int imgH = this.Image.Height;

                // Căn giữa theo chiều ngang
                int imgX = (this.Width - imgW) / 2;
                int imgY = 8; // Cách lề trên một chút

                // Vẽ ảnh
                g.DrawImage(this.Image, imgX, imgY, imgW, imgH);

                // Vẽ Text (Giá tiền) nằm ở dưới ảnh
                // Tạo một hình chữ nhật bao quanh phần dưới của nút
                int textY = imgY + imgH + 2;
                Rectangle textRect = new Rectangle(0, textY, this.Width, this.Height - textY);

                // Vẽ chữ vào giữa hình chữ nhật đó
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
            }
            else
            {
                // --- TRƯỜNG HỢP KHÔNG CÓ ẢNH (NÚT THƯỜNG) ---
                // Vẽ chữ nằm chính giữa nút
                TextRenderer.DrawText(g, this.Text, this.Font, rect, this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        // =========================================================
        // HÀM HỖ TRỢ VẼ BO TRÒN
        // =========================================================
        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2.0F;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // =========================================================
        // XỬ LÝ SỰ KIỆN CHUỘT (ĐỂ ĐỔI MÀU)
        // =========================================================
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Invalidate(); // Yêu cầu vẽ lại
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            Invalidate();
        }
    }
}
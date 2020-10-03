﻿using System;

namespace Simplex.Gui.Control
{
	/// <summary>
	/// CheckBox with label.
	/// </summary>
	[Xml.XmlControl]
	public class LabeledCheckBox : ControlBase
    {
        private readonly CheckBox m_CheckBox;
        private readonly Label m_Label;

		/// <summary>
		/// Invoked when the control has been checked.
		/// </summary>
		[Xml.XmlEvent]
		public event GuiEventHandler<EventArgs> Checked;

		/// <summary>
		/// Invoked when the control has been unchecked.
		/// </summary>
		[Xml.XmlEvent]
		public event GuiEventHandler<EventArgs> UnChecked;

		/// <summary>
		/// Invoked when the control's check has been changed.
		/// </summary>
		[Xml.XmlEvent]
		public event GuiEventHandler<EventArgs> CheckChanged;

		/// <summary>
		/// Indicates whether the control is checked.
		/// </summary>
		[Xml.XmlProperty]
		public bool IsChecked { get { return m_CheckBox.IsChecked; } set { m_CheckBox.IsChecked = value; } }

		/// <summary>
		/// Label text.
		/// </summary>
		[Xml.XmlProperty]
		public string Text { get { return m_Label.Text; } set { m_Label.Text = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabeledCheckBox"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledCheckBox(ControlBase parent)
            : base(parent)
        {
            m_CheckBox = new CheckBox(this);
            m_CheckBox.IsTabable = false;
            m_CheckBox.CheckChanged += OnCheckChanged;

            m_Label = new Label(this);
			m_Label.Clicked += delegate(ControlBase Control, ClickedEventArgs args) { m_CheckBox.Press(Control); };
            m_Label.IsTabable = false;

            IsTabable = false;
        }

		protected override Size Measure(Size availableSize)
		{
			Size labelSize = m_Label.DoMeasure(availableSize);
			Size radioButtonSize = m_CheckBox.DoMeasure(availableSize);

			return new Size(labelSize.Width + 4 + radioButtonSize.Width, Math.Max(labelSize.Height, radioButtonSize.Height));
		}

		protected override Size Arrange(Size finalSize)
		{
			if (m_CheckBox.MeasuredSize.Height > m_Label.MeasuredSize.Height)
			{
				m_CheckBox.DoArrange(new Rectangle(0, 0, m_CheckBox.MeasuredSize.Width, m_CheckBox.MeasuredSize.Height));
				m_Label.DoArrange(new Rectangle(m_CheckBox.MeasuredSize.Width + 4, (m_CheckBox.MeasuredSize.Height - m_Label.MeasuredSize.Height) / 2, m_Label.MeasuredSize.Width, m_Label.MeasuredSize.Height));
			}
			else
			{
				m_CheckBox.DoArrange(new Rectangle(0, (m_Label.MeasuredSize.Height - m_CheckBox.MeasuredSize.Height) / 2, m_CheckBox.MeasuredSize.Width, m_CheckBox.MeasuredSize.Height));
				m_Label.DoArrange(new Rectangle(m_CheckBox.MeasuredSize.Width + 4, 0, m_Label.MeasuredSize.Width, m_Label.MeasuredSize.Height));
			}

			return MeasuredSize;
		}

		/// <summary>
		/// Handler for CheckChanged event.
		/// </summary>
		protected virtual void OnCheckChanged(ControlBase control, EventArgs Args)
        {
            if (m_CheckBox.IsChecked)
            {
                if (Checked != null)
					Checked.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (UnChecked != null)
					UnChecked.Invoke(this, EventArgs.Empty);
            }

            if (CheckChanged != null)
				CheckChanged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        /// True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            base.OnKeySpace(down);
            if (!down) 
                m_CheckBox.IsChecked = !m_CheckBox.IsChecked; 
            return true;
        }
    }
}

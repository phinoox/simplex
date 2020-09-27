﻿using System;
using Simplex.Gui.Control;

namespace Simplex.Gui.Control.Internal
{
	public class WindowTitleBar : Dragger
	{
		private readonly Label m_Title;
		private readonly CloseButton m_CloseButton;

		public Label Title { get { return m_Title; } }
		public CloseButton CloseButton { get { return m_CloseButton; } }

		public WindowTitleBar(ControlBase parent)
			: base(parent)
		{
			m_Title = new Label(this);
			m_Title.Alignment = Alignment.Left | Alignment.CenterV;

			m_CloseButton = new CloseButton(this, parent as Window);
			m_CloseButton.IsTabable = false;
			m_CloseButton.Name = "closeButton";

			Target = parent;
		}

		protected override Size Measure(Size availableSize)
		{
			m_Title.DoMeasure(availableSize);

			if (!m_CloseButton.IsCollapsed)
				m_CloseButton.DoMeasure(availableSize);

			return availableSize;
		}

		protected override Size Arrange(Size finalSize)
		{
			m_Title.DoArrange(new Rectangle(8, 0, m_Title.MeasuredSize.Width, finalSize.Height));

			if (!m_CloseButton.IsCollapsed)
			{
				int closeButtonSize = finalSize.Height;
				m_CloseButton.DoArrange(new Rectangle(finalSize.Width - 6 - closeButtonSize, 0, closeButtonSize, closeButtonSize));
			}

			return finalSize;
		}
	}
}

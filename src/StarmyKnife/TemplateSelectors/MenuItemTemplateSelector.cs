﻿using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

namespace StarmyKnife.TemplateSelectors;

public class MenuItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate GlyphDataTemplate { get; set; }

    public DataTemplate ImageDataTemplate { get; set; }

    public DataTemplate IconDataTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is HamburgerMenuGlyphItem)
        {
            return GlyphDataTemplate;
        }

        if (item is HamburgerMenuImageItem)
        {
            return ImageDataTemplate;
        }

        if (item is HamburgerMenuIconItem)
        {
            return IconDataTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}

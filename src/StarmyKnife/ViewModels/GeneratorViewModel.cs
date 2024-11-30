﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Helpers;
using StarmyKnife.UserControls.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace StarmyKnife.ViewModels;

public class GeneratorViewModel : SinglePluginPageViewModelBase<IGenerator>
{
    private int _numberOfGeneration;
    private string _output;

    public GeneratorViewModel(IPluginLoaderService pluginLoader, IEventAggregator eventAggregator) : base(pluginLoader, eventAggregator)
    {
        NumberOfGeneration = 1;
        GenerateCommand = new DelegateCommand(Generate);
    }

    public int NumberOfGeneration
    {
        get
        {
            return _numberOfGeneration;
        }
        set
        {
            if (value < 0)
            {
                Errors.SetErrorsIfChanged(nameof(NumberOfGeneration), Properties.Resources.Common_MustBeGreaterThanOrEqualsTo, Properties.Resources.Generator_NumberOfGeneration, 1);
            }
            else
            {
                Errors.ClearErrors(nameof(NumberOfGeneration));
                SetProperty(ref _numberOfGeneration, value);
            }

        }
    }

    public string Output
    {
        get { return _output; }
        set { SetProperty(ref _output, value); }
    }

    public DelegateCommand GenerateCommand { get; }

    private void Generate()
    {
        var sb = new StringBuilder();
        var plugin = (IGenerator)SelectedPlugin.Plugin;
        var parameters = PluginBox.Parameters;

        try
        {
            for (int i = 0; i < NumberOfGeneration; i++)
            {
                var result = plugin.Generate(parameters);
                sb.AppendLine(result);
            }

            Output = sb.ToString();
        }
        catch (Exception ex)
        {
            Output = "";
            MessageBox.Show(string.Format(Properties.Resources.Generator_ErrorWhileGeneratingString, ex.Message));
        }

    }
}

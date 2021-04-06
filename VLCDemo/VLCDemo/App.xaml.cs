﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VLCDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new MediaPlayerElementPage(); //Font asset not found FontAwesome5Regular.otf'
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

#region Copyright
// <copyright file="MvxBindingActivityView.cs" company="Cirrious">
// (c) Copyright Cirrious. http://www.cirrious.com
// This source is subject to the Microsoft Public License (Ms-PL)
// Please see license.txt on http://opensource.org/licenses/ms-pl.html
// All other rights reserved.
// </copyright>
// 
// Project Lead - Stuart Lodge, Cirrious. http://www.cirrious.com
#endregion

using System;
using Android.Views;
using Cirrious.MvvmCross.Android.Views;
using Cirrious.MvvmCross.Binding.Android.Binders;
using Cirrious.MvvmCross.Binding.Android.Interfaces.Views;
using Cirrious.MvvmCross.Interfaces.ViewModels;

namespace Cirrious.MvvmCross.Binding.Android.Views
{
    public abstract class MvxBindingActivityView<TViewModel>
        : MvxActivityView<TViewModel>
          , IMvxBindingActivity
        where TViewModel : class, IMvxViewModel
    {
        #region Code shared across all binding activities - I hate this cut and paste

        public override LayoutInflater LayoutInflater
        {
            get
            {
                throw new InvalidOperationException("LayoutInflater must not be accessed directly in MvxBindingActivityView - use IMvxBindingActivity.BindingInflate or IMvxBindingActivity.NonBindingInflate instead");
            }
        }

        public View BindingInflate(int resourceId, ViewGroup viewGroup)
        {
            return BindingInflate(ViewModel, resourceId, viewGroup);
        }

        public View BindingInflate(object source, int resourceId, ViewGroup viewGroup)
        {
            return CommonInflate(
                resourceId, 
                viewGroup, 
                (layoutInflator) => new MvxBindingLayoutInflatorFactory(source, layoutInflator));
        }

        public View NonBindingInflate(int resourceId, ViewGroup viewGroup)
        {
            return CommonInflate(
                resourceId,
                viewGroup,
                (layoutInflator) => null);
        }

        public override void SetContentView(int layoutResId)
        {
            var view  = BindingInflate(ViewModel, layoutResId, null);
            SetContentView(view);
        }

        private View CommonInflate(int resourceId, ViewGroup viewGroup, Func<LayoutInflater, MvxBindingLayoutInflatorFactory> factoryProvider)
        {
            var layoutInflator = base.LayoutInflater;
            using (var clone = layoutInflator.CloneInContext(this))
            {
                using (var factory = factoryProvider(clone))
                {
                    if (factory != null)
                        clone.Factory = factory;
                    var toReturn = clone.Inflate(resourceId, viewGroup);
                    if (factory != null)
                    {
                        factory.StoreBindings(toReturn);
                    }
                    return toReturn;
                }
            }
        }

        #endregion
    }
}
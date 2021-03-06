﻿using Android.App;
using Android.Content;
using Android.Service.QuickSettings;
using Android.Views.Accessibility;
using Android.Widget;

using System;

using static GAButtonMapper.ETC;

namespace GAButtonMapper
{
    [Service(Name = "com.lgplus.safechargetoggle.QuickToggleTileService",
             Permission = Android.Manifest.Permission.BindQuickSettingsTile,
             Label = "@string/TileService_Title",
             Icon = "@drawable/qtile_icon")]
    [IntentFilter(new[] { ActionQsTile, ActionQsTilePreferences })]
    public class QuickToggleTileService : TileService
    {
        private readonly string mappingPrefId = "EnableMapping";

        public override void OnTileAdded()
        {
            base.OnTileAdded();
        }

        public override void OnStartListening()
        {
            base.OnStartListening();

            try
            {
                if (acm == null)
                {
                    acm = GetSystemService("accessibility") as AccessibilityManager;
                }

                if (!acm.IsEnabled)
                {
                    QsTile.State = TileState.Inactive;

                    QsTile.UpdateTile();

                    return;
                }

                QsTile.State = sharedPreferences.GetBoolean(mappingPrefId, false) ? TileState.Active : TileState.Inactive;

                QsTile.UpdateTile();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }

        public override void OnClick()
        {
            try
            {
                if (acm == null)
                {
                    acm = GetSystemService("accessibility") as AccessibilityManager;
                }

                if (!acm.IsEnabled)
                {
                    Toast.MakeText(this, Resource.String.TileService_UnableMessage, ToastLength.Short).Show();

                    return;
                }

                base.OnClick();

                if (sharedPreferences.GetBoolean(mappingPrefId, false))
                {
                    sharedPreferences.Edit().PutBoolean(mappingPrefId, false).Apply();

                    isMappingEnable = false;
                    QsTile.State = TileState.Inactive;

                    Toast.MakeText(this, Resource.String.TileService_DisableMessage, ToastLength.Short).Show();
                }
                else
                {
                    sharedPreferences.Edit().PutBoolean(mappingPrefId, true).Apply();

                    QsTile.State = TileState.Active;

                    Toast.MakeText(this, Resource.String.TileService_EnableMessage, ToastLength.Short).Show();

                    isMappingEnable = true;

                    if (!isRun)
                    {
                        monitoringMethod();
                    }
                }

                QsTile.UpdateTile();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }
    }
}
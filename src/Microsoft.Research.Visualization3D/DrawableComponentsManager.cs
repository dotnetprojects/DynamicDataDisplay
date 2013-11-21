using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Visualization3D.MainLoops;

namespace Microsoft.Research.Visualization3D
{
    public class DrawableComponentsManager
    {
        private List<DrawableComponent> providers;
        private List<DrawableComponent> backgroundProviders;

        public DrawableComponentsManager()
        {
            providers = new List<DrawableComponent>();
            backgroundProviders = new List<DrawableComponent>();
        }

        public void Initialize()
        {
            foreach (DrawableComponent provider in providers)
            {
                if (provider.IsEnabled)
                    provider.Initialize();
            }
            foreach (DrawableComponent provider in backgroundProviders)
            {
                provider.Initialize();
            }
        }

        public void Update(TimeEntity time)
        {
            foreach (DrawableComponent provider in providers)
            {
                if (provider.IsEnabled)
                    provider.Update(time);
            }
            foreach (DrawableComponent provider in backgroundProviders)
            {
                provider.Update(time);
            }
        }

        public void Draw(TimeEntity time)
        {
            foreach (DrawableComponent provider in providers)
            {
                if (provider.IsEnabled)
                    provider.Draw(time);
            }
            foreach (DrawableComponent provider in backgroundProviders)
            {
                provider.Draw(time);
            }
        }

        public void AddProvider(DrawableComponent newProvider, bool enable)
        {
            providers.Add(newProvider);
            if (enable)
            {
                foreach (DrawableComponent provider in providers)
                {
                    provider.IsEnabled = false;
                }
                newProvider.IsEnabled = true;
            }
            else
            {
                newProvider.IsEnabled = false;
            }
        }

        public void AddBackgroundProvider(DrawableComponent newBackgroundProvider)
        {
            backgroundProviders.Add(newBackgroundProvider);
            newBackgroundProvider.IsEnabled = true;
            newBackgroundProvider.Initialize();
        }

        public DrawableComponent GetProviderByType<T>()
        {
            DrawableComponent result = providers.Find(p => p is T);
            if (result != null)
                return result;
            else
            {
                result = backgroundProviders.Find(p => p is T);
                if (result != null)
                    return result;
            }
            throw new ArgumentException("Invalid Provider Type");
        }

        public DrawableComponent SetCurrent<T>()
        {
            DrawableComponent provider = providers.Find(p => p is T);
            if (provider != null && !provider.IsEnabled)
            {
                foreach (DrawableComponent otherProvdider in providers)
                {
                    otherProvdider.IsEnabled = false;
                }
                provider.IsEnabled = true;
            }
            return provider;
        }

        
       

    }
}

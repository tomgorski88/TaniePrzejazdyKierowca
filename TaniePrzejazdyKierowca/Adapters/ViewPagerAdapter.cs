using AndroidX.Fragment.App;
using System.Collections.Generic;

namespace TaniePrzejazdyKierowca.Adapters
{
    public class ViewPagerAdapter : FragmentPagerAdapter
    {
        public List<Fragment> Fragments { get; set; }
        public List<string> fragmentNames { get; set; }

        public ViewPagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {
            Fragments = new List<Fragment>();
            fragmentNames = new List<string>();
        }

        public void AddFragment(Fragment fragment, string name)
        {
            Fragments.Add(fragment);
            fragmentNames.Add(name);
        }

        public override int Count
        {
            get
            {
                return Fragments.Count;
            }
        }

        public override Fragment GetItem(int position)
        {
            return Fragments[position];
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace NC
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        // VARIABLE TO SCALE BAR SIZE DEPENDING ON STAT (HIGHER STAT = LONGER BAR ACROSS THE SCREEN)
        // SECONDARY BAR BEHIND MAY BAR FOR POLISH EFFECT (YELLOW BAR THAT SHOWS HOW STAMINA AN ACTION/DAMAGE TAKES AWAY)

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;
        }
    }    
}

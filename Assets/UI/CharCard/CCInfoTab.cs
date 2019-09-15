using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum cctab_page { active, passive, equip }
public class CCInfoTab : MonoBehaviour {
    private cctab_page m_currentPage = cctab_page.active;
    public cctab_page TabPage {
        get { return m_currentPage; }
        set {
            m_currentPage = value;
            switch (value) {
                case cctab_page.active:
                    transform.Find ("act_skill_tab/on").gameObject.SetActive (true);
                    transform.Find ("pass_skill_tab/on").gameObject.SetActive (false);
                    transform.Find ("equiped/on").gameObject.SetActive (false);

                    transform.Find ("active_tabs").gameObject.SetActive (true);
                    transform.Find ("passive_tabs").gameObject.SetActive (false);
                    transform.Find ("equi_tabs").gameObject.SetActive (false);
                    break;
                case cctab_page.passive:
                    transform.Find ("act_skill_tab/on").gameObject.SetActive (false);
                    transform.Find ("pass_skill_tab/on").gameObject.SetActive (true);
                    transform.Find ("equiped/on").gameObject.SetActive (false);

                    transform.Find ("active_tabs").gameObject.SetActive (false);
                    transform.Find ("passive_tabs").gameObject.SetActive (true);
                    transform.Find ("equi_tabs").gameObject.SetActive (false);
                    break;
                case cctab_page.equip:
                    transform.Find ("act_skill_tab/on").gameObject.SetActive (false);
                    transform.Find ("pass_skill_tab/on").gameObject.SetActive (false);
                    transform.Find ("equiped/on").gameObject.SetActive (true);

                    transform.Find ("active_tabs").gameObject.SetActive (false);
                    transform.Find ("passive_tabs").gameObject.SetActive (false);
                    transform.Find ("equi_tabs").gameObject.SetActive (true);
                    break;

            }
        }
    }

}
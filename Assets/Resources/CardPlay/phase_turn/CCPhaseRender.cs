using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.ProtoMod;
using UnityEditor;
using UnityEngine;
public class CCPhaseRender : MonoBehaviour {
    public GameObject IconBlock;
    public Animator cAnimator;
    public string dist {
        get {
            return "l";
        }
    }
    public bool is_first_attack = false;
    private void Start() {
        if (this.IconBlock == null) {
            this.IconBlock = this.transform.Find("set_a/char_f").gameObject;
        }
        if (this.cAnimator == null) {
            this.cAnimator = this.gameObject.GetComponent<Animator>();
        }
    }

    public IEnumerator InitCCImg(AssetBundle abs, CardSet Cs) {
        if (abs == null) {
            yield return false;
        }

        if (this.IconBlock == null) {
            yield return false;
        } else {
            Texture2D tas = abs.LoadAsset(Cs.artifact_image.name)as Texture2D;
            var sprit = IconBlock.GetComponent<SpriteRenderer>();
            sprit.sprite = Sprite.Create(tas,
                new Rect(0, 0, tas.width, tas.height),
                new Vector2(0.5f, 0.5f)
            );
            float orig_ratio = ((float)tas.height / (float)tas.width) /
                ((float)Cs.artifact_image.height / (float)Cs.artifact_image.width);
            IconBlock.GetComponent<Transform>().localScale *= new Vector2(orig_ratio * 1.2f, 1 * 1.2f);
        }
        yield return true;
    }
    /// <summary>
    ///     ready -> pick-up -> ok_state -> start-dice
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="event_phase"></param>
    /// <returns></returns>
    public IEnumerator PlayAnimation(EventHookPhase phase, EventHookType event_phase) {
        if (this.cAnimator == null) {
            yield return false;
        }
        Debug.Log(phase);
        switch (phase) {

            case EventHookPhase.MoveCardDropPhase: //30
                if (event_phase == EventHookType.Before) {
                    this.cAnimator.Play("move");
                    yield return new WaitForSeconds(1.5f);
                }
                break;
            case EventHookPhase.FinishMovePhase: //50
                if (event_phase == EventHookType.Before) {
                    this.cAnimator.Play("move_left");
                    yield return new WaitForSeconds(1.5f);
                }
                break;

            case EventHookPhase.AttackCardDropPhasePA: //80
                if (event_phase == EventHookType.Before) {
                    if (is_first_attack) {
                        this.cAnimator.Play("atk_ready");
                        yield return new WaitForSeconds(1.5f);
                        this.cAnimator.Play("atk_pick_up_" + dist);
                    } // atk
                    else {
                        this.cAnimator.Play("def_ready");
                    } // def
                } else if (event_phase == EventHookType.After) {
                    if (is_first_attack) {
                        this.cAnimator.Play("atk_ok_state_" + dist);
                    } // atk - end of picking
                }
                yield return new WaitForSeconds(1.5f);
                break;
            case EventHookPhase.DefenceCardDropPhasePA: //90
                if (event_phase == EventHookType.Before) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("def_pick_up");
                    } //def
                } else if (event_phase == EventHookType.After) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("def_ok_state");
                    } //def
                }
                yield return new WaitForSeconds(1.5f);
                break;

            case EventHookPhase.AttackCardDropPhasePB:
                if (event_phase == EventHookType.Before) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("atk_ready");
                        yield return new WaitForSeconds(1.5f);
                        this.cAnimator.Play("atk_pick_up_" + dist);
                    } // atk
                    else {
                        this.cAnimator.Play("def_ready");
                    } // def
                } else if (event_phase == EventHookType.After) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("atk_ok_state_" + dist);
                    } // atk - end of picking
                }
                yield return new WaitForSeconds(1.5f);
                break;
            case EventHookPhase.DefenceCardDropPhasePB:
                if (event_phase == EventHookType.Before) {
                    if (is_first_attack) {
                        this.cAnimator.Play("def_pick_up");
                    } //def
                } else if (event_phase == EventHookType.After) {
                    if (is_first_attack) {
                        this.cAnimator.Play("def_ok_state");
                    } //def
                }
                yield return new WaitForSeconds(1.5f);
                break;

            case EventHookPhase.DetermineBattlePointPhasePA: //100
            case EventHookPhase.DetermineBattlePointPhasePB: //160
                if (event_phase == EventHookType.Before) {
                    if (is_first_attack)this.cAnimator.Play("atk_start_dice"); // atk
                    else this.cAnimator.Play("def_start_dice");
                    yield return new WaitForSeconds(1.5f);
                }
                break;
            default:
                break;
        }
        yield return true;
    }
    public void PauseAnim(string name) {
        Debug.Log("layer:" + name);
        if (name != "") {
            // this.cAnimator.speed = 0f;
        }
    }
    public void ContAnim(string name) {
        Debug.Log("layer:" + name);
        // this.cAnimator.speed = 1f;
    }

    public void test_PlayAnimation(int phase, int event_phase) {
        StartCoroutine(PlayAnimation(
            (EventHookPhase)Enum.ToObject(typeof(EventHookPhase), phase * 10),
            (EventHookType)Enum.ToObject(typeof(EventHookType), event_phase)));
    }
}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(CCPhaseRender))]
public class CCPhaseRender_Editor : Editor {
    int CD = 0;
    int sider_phase = 10;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        CCPhaseRender d = (CCPhaseRender)target;
        sider_phase = EditorGUILayout.IntSlider("sider-phase", sider_phase, 3, 19);
        CD = EditorGUILayout.IntSlider("Type", CD, 0, 3);

        if (GUILayout.Button("Test Move Phase")) {
            d.test_PlayAnimation(sider_phase, CD);
        }

        if (GUILayout.Button("Test Atk Phase")) {
            d.test_PlayAnimation(1, CD);
        }

        if (GUILayout.Button("Test Def Phase")) {
            d.test_PlayAnimation(2, CD);
        }

    }
}

#endif
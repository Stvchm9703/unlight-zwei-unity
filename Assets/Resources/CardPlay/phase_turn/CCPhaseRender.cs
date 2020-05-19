using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.ProtoMod.GameDuelService;
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

    public IEnumerator InitCCImg2(CardSetPack Cs) {

        if (this.IconBlock == null) {
            yield return false;
        } else {
            Texture2D tas = Cs.artifact_image_t2;
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

    public IEnumerator MovePhaseAnimation(bool in_out) {
        if (in_out) {
            this.cAnimator.Play("move");
            yield return new WaitForSeconds(1.2f);
        } else {
            this.cAnimator.Play("move_left");
            yield return new WaitForSeconds(1.2f);
        }
        yield return true;
    }
    public IEnumerator AttackerPhaseAnimation(EventHookPhase phase, EventHookType type, RangeType range) {
        string dist = "";
        switch (range) {
            case RangeType.Long:
            case RangeType.Middle:
                dist = "l";
                break;
            case RangeType.Short:
                dist = "s";
                break;
        }
        switch (phase) {
            case EventHookPhase.AttackCardDropPhase:
                if (type == EventHookType.Before) {
                    this.cAnimator.Play("atk_ready");
                    yield return new WaitForSeconds(1.5f);
                    this.cAnimator.Play("atk_pick_up_" + dist);
                } else if (type == EventHookType.After) {
                    this.cAnimator.Play("atk_ok_state_" + dist);
                }
                break;
            case EventHookPhase.DetermineBattlePointPhase:
                if (type == EventHookType.Before) {
                    this.cAnimator.Play("atk_start_dice"); // atk
                }
                break;
        }
        yield return new WaitForSeconds(1.5f);
    }
    public IEnumerator DeferencePhaseAnimation(EventHookPhase phase, EventHookType type, RangeType range) {

        switch (phase) {
            case (EventHookPhase.AttackCardDropPhase):
                this.cAnimator.Play("def_ready");
                break;
            case (EventHookPhase.DefenceCardDropPhase):
                if (type == EventHookType.Before)
                    this.cAnimator.Play("def_pick_card");
                else if (type == EventHookType.After)
                    this.cAnimator.Play("def_ok_state");
                break;
            case (EventHookPhase.DetermineBattlePointPhase): //100
                this.cAnimator.Play("def_start_dice"); // atk
                break;
        }
        yield return new WaitForSeconds(1.5f);
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

            // Move-phase in 
            case EventHookPhase.MoveCardDropPhase:
                { //30
                    if (event_phase == EventHookType.Before) {
                        this.cAnimator.Play("move");
                        yield return new WaitForSeconds(1.5f);
                    }
                    break;
                }
                // Move-phase out
            case EventHookPhase.FinishMovePhase:
                { //50
                    if (event_phase == EventHookType.Before) {
                        this.cAnimator.Play("move_left");
                        yield return new WaitForSeconds(1.5f);
                    }
                    break;
                }

                // wiede
            case EventHookPhase.AttackCardDropPhase:
                { //80
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
                }
            case EventHookPhase.DefenceCardDropPhase: //90
                if (event_phase == EventHookType.Before) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("atk_ready");
                        yield return new WaitForSeconds(1.5f);
                        this.cAnimator.Play("atk_pick_up_" + dist);
                    } else {
                        this.cAnimator.Play("def_ready");
                    } // def
                } else if (event_phase == EventHookType.After) {
                    if (!is_first_attack) {
                        this.cAnimator.Play("def_ok_state");
                    } else {
                        this.cAnimator.Play("atk_ok_state_" + dist);
                    }
                }
                yield return new WaitForSeconds(1.5f);
                break;

                // case EventHookPhase.AttackCardDropPhase:
                //     if (event_phase == EventHookType.Before) {
                //         if (!is_first_attack) {
                //             this.cAnimator.Play("atk_ready");
                //             yield return new WaitForSeconds(1.5f);
                //             this.cAnimator.Play("atk_pick_up_" + dist);
                //         } // atk
                //         else {
                //             this.cAnimator.Play("def_ready");
                //         } // def
                //     } else if (event_phase == EventHookType.After) {
                //         if (!is_first_attack) {
                //             this.cAnimator.Play("atk_ok_state_" + dist);
                //         } // atk - end of picking
                //     }
                //     yield return new WaitForSeconds(1.5f);
                //     break;
                // case EventHookPhase.DefenceCardDropPhase:
                //     if (event_phase == EventHookType.Before) {
                //         if (is_first_attack) {
                //             this.cAnimator.Play("def_pick_up");
                //         } //def
                //     } else if (event_phase == EventHookType.After) {
                //         if (is_first_attack) {
                //             this.cAnimator.Play("def_ok_state");
                //         } //def
                //     }
                //     yield return new WaitForSeconds(1.5f);
                //     break;

            case EventHookPhase.DetermineBattlePointPhase: //100
                // case EventHookPhase.DetermineBattlePointPhase: //160
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

    public void test_PlayAnimation(EventHookPhase phase, EventHookType type) {
        StartCoroutine(PlayAnimation(phase, type));
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

        if (GUILayout.Button("Test Move Phase in")) {
            d.test_PlayAnimation(EventHookPhase.MoveCardDropPhase, EventHookType.Before);
        }
        if (GUILayout.Button("Test Move Phase out")) {
            d.test_PlayAnimation(EventHookPhase.FinishMovePhase, EventHookType.Before);
        }

        if (GUILayout.Button("Test ATK Phase in")) {
            d.test_PlayAnimation(EventHookPhase.AttackCardDropPhase, EventHookType.Before);
        }
        if (GUILayout.Button("Test ATK Phase out")) {
            d.test_PlayAnimation(EventHookPhase.AttackCardDropPhase, EventHookType.After);
        }

        if (GUILayout.Button("Test DEF Phase in")) {
            d.test_PlayAnimation(EventHookPhase.DefenceCardDropPhase, EventHookType.Before);
        }
        if (GUILayout.Button("Test DEF Phase out")) {
            d.test_PlayAnimation(EventHookPhase.DefenceCardDropPhase, EventHookType.After);
        }

        if (GUILayout.Button("Test Dice Phase ")) {
            d.test_PlayAnimation(EventHookPhase.DetermineBattlePointPhase, EventHookType.Before);
        }

    }
}

#endif
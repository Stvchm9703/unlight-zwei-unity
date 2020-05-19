using System.Threading;
using ULZAsset.ProtoMod.GameDuelService;
using UnityEditor;
using UnityEngine;
#if (UNITY_EDITOR) 
[CustomEditor(typeof(MainCtrlComp))]
public class MainCtrlComp_Editor : Editor {
    int CD = 0, TestStatusID = 1, TestStatusCD = 0;
    int sider_phase = 10;
    bool open_a = true;

    // PhaseTurnCtlSingal
    Test_EventHookPhase PhaseSingal_HookPhase;
    EventHookType PhaseSingal_type;

    // Range 
    RangeType TestRange;
    PlayerSide TestFirstAtk;

    private void Start() {

    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        MainCtrlComp d = (MainCtrlComp)target;
        // ----------------------------------------------
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Character Card Asset");
        if (GUILayout.Button("Test Load Asset ")) {
            d.TestLoad();
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Open Self CC Info Panel")) {
            d.OpenCCInfoPanel(1);
        }
        if (GUILayout.Button("Test Open Duel CC Info Panel")) {
            d.OpenCCInfoPanel(0);
        }
        EditorGUILayout.EndHorizontal();
        // ----------------------------------------------
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Status Control");

        EditorGUILayout.BeginHorizontal();
        TestStatusID = EditorGUILayout.IntField("Test ID", TestStatusID);
        TestStatusCD = EditorGUILayout.IntField("Test CD", TestStatusCD);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Insert Status into Self ")) {
            d.InsertStatusToSelf(TestStatusID, TestStatusCD);
        }
        if (GUILayout.Button("Insert Status into Duel")) {
            d.InsertStatusToDuel(TestStatusID, TestStatusCD);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Status into Self ")) {
            d.RemoveStatusToSelf(TestStatusCD);
        }
        if (GUILayout.Button("Remove Status into Duel")) {
            d.RemoveStatusToDuel(TestStatusCD);
        }
        EditorGUILayout.EndHorizontal();
        // ----------------------------------------------
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("EventCard Block Test");
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Card Set")) {
            d.SelfEventCardSetTest();
            d.DuelEventCardSetTest();
        }
        if (GUILayout.Button("Gen Card Set with dt")) {
            d.TestGenerateCardData();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Draw 5 Card")) {
            for (int i = 0; i < 5; i++) {
                d.SelfDrawCardTest();
                d.DuelDrawCardTest();
            }
        }

        if (GUILayout.Button("Self Ready")) {
            d.SelfFlipOpenTest();
        }

        if (GUILayout.Button("Duel Drop Card")) {
            d.DuelRamdomPush();
        }
        if (GUILayout.Button("Duel Flip Card")) {
            d.DuelFlipOpenTest();
        }
        // EditorGUILayout.EndHorizontal();

        // EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Card Drop End Phase")) {
            d.SelfCardToDestroy();
            d.DuelCardToDestroy();
        }

        // ----------------------------------------------
        // ----------------------------------------------

        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Range Changing Test");
        // EditorGUILayout.BeginHorizontal();
        TestRange = (RangeType)EditorGUILayout.EnumPopup("Range", TestRange);
        if (GUILayout.Button("Run Change")) {
            d.PlayChangeRange(TestRange);
        }
        // ----------------------------------------------

        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Player Option Phase Turn");
        TestFirstAtk = (PlayerSide)EditorGUILayout.EnumPopup("First Atk", TestFirstAtk);
        PhaseSingal_HookPhase = (Test_EventHookPhase)EditorGUILayout.EnumPopup("Test Flag", PhaseSingal_HookPhase);
        PhaseSingal_type = (EventHookType)EditorGUILayout.EnumPopup("Test Type", PhaseSingal_type);

        if (GUILayout.Button("Run Change")) {
            d.PlayerPhaseTurn(
                Test_EventHookPhaseCast.ToEventHookPhase(PhaseSingal_HookPhase),
                PhaseSingal_type,
                TestFirstAtk,
                TestRange);
        }
        // ----------------------------------------------

        //  Whole Flow 
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Whole Game Flow");

        // Game Init 
        if (GUILayout.Button("Game Init")) {
            d.TestGameInitPhase();
        }

        // Draw Phase 
        if (GUILayout.Button("Draw Phase")) {
            Debug.Log("Start Draw Phase");
            d.TestGameDrawPhaseUI();
            Debug.Log("End Draw Phase");

        }

        // Move Phase 
        if (GUILayout.Button("Move Phase")) {
            Debug.Log("Start Move Phase");
            d.TestMovePhaseUI();
        }

        // Attack 
        if (GUILayout.Button("Host Attack Phase")) {
            // Start Attack Phase
            d.TestHostAtkPhaseUI();
            // dice 
        }

        // Dice Phase / Determine-battle-point
        if (GUILayout.Button("Dueler Attack Phase")) {
            d.TestDuelAtkPhaseUI();
         }

    }
}

#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateTransitionDiagram {

    //DISCLAIMER: This thing only currently works with level 10. I can make it more extensible in the future 
    // it assumes that there are exactly 1 mirror and 1 mimic. The state transition diagram and checks are hardcoded 

    /* 
     A State has a count associated with the state (how many players reached this state) 
     A State has a number of quits, restarts, and wins that occurred at this state
     A State has a dictionary that represents its possible transitions to other states

    Possible States: 
        1: Where Mirror.col <= 3
        2: Where Mimic.c >= 9
        3: Where Mirror.row == 7 && Mirror.col == 3
             &&  Player.row == 1 && Player.col == 7
        
    Possible End Results: 
        2: Restart
        3: Level Select
        4: Win
        5: Quit
        6: Alternate Win
         */

    private class State {
        public Dictionary<int, State> transitions = new Dictionary<int, State>();
        public int count = 0;
        public int restarts = 0;
        public int levelSelect = 0; 
        public int quits = 0;
        public int wins = 0;
        public int weirdwins = 0; 
    }

    static State StateTransition; 


    // Use this for initialization
    public static void Init() {
        /*                 3 -- 2
         *               /
         *             1
         *          /    \
         *    Start         2 -- 3
         *          \
         *             2 -- 1 -- 3
         */

        StateTransition = new State();
        StateTransition.transitions.Add(1, new State());
        StateTransition.transitions.Add(2, new State());

        StateTransition.transitions[1].transitions.Add(2, new State());
        StateTransition.transitions[1].transitions.Add(3, new State());

        StateTransition.transitions[1].transitions[3].transitions.Add(2, new State());
        StateTransition.transitions[1].transitions[2].transitions.Add(3, new State());

        StateTransition.transitions[2].transitions.Add(1, new State());
        StateTransition.transitions[2].transitions[1].transitions.Add(3, new State()); 
    }

    // Update is called once per frame
    public static void MakeDiagram(List<List<DynamicState>> states, List<int> endResults) {
        Debug.Log(states.Count);
        Debug.Log(endResults.Count);
        if(states == null) {
            return; 
        }
        State currentState;
        bool didWin; 

        for (int i = 0; i < states.Count; i++) {
            StateTransition.count++;
            currentState = StateTransition;
            didWin = false; 
            
            foreach (DynamicState state in states[i]) {
                if(state == null) {
                    continue; 
                }
                //check to see if you can transition. If so, do 
                if (state.mirrorPositions.Count > 0 && state.mimicPositions.Count > 0) {
                    if (state.mirrorPositions[0].col <= 3 && currentState.transitions.ContainsKey(1)) {
                        currentState = currentState.transitions[1];
                        currentState.count++;
                    }
                    if (state.mimicPositions[0].col >= 9 && currentState.transitions.ContainsKey(2)) {
                        currentState = currentState.transitions[2];
                        currentState.count++;
                    }
                    if (state.mirrorPositions[0].row == 7 &&
                              state.mirrorPositions[0].col == 3 &&
                              state.playerPosition.row == 1 &&
                              state.playerPosition.col == 7 &&
                              currentState.transitions.ContainsKey(3)) {
                        currentState = currentState.transitions[3];
                        currentState.count++;
                    }
                    if (state.mimicPositions[0].col == 8 && 
                        state.mimicPositions[0].row == 7 && 
                        state.mirrorPositions[0].col == 1 && 
                        state.mirrorPositions[0].row == 1) {
                        currentState.wins++;
                        didWin = true;
                    }
                }
                
            }

            if (!didWin) {
                currentState.levelSelect++; 
            }
            
            
        }

        PrintDiagram(); 
    }

    static string PrintState(int[] a) {
        State currentState = StateTransition;
        foreach (int i in a) {
            currentState = currentState.transitions[i];
        }
        return currentState.count + " {" + currentState.levelSelect + ", " + currentState.wins +  "}"; 
    }

    public static void PrintDiagram() {
        Debug.Log(
            "        " + PrintState(new int[]{1,3}) + "  -- " + PrintState(new int[] { 1, 3, 2 }) + " \n" +
            "       / \n" +
            "      " + PrintState(new int[] { 1 }) + " \n" +
            "     / \\ \n" +
            PrintState(new int[] { }) + "    " +PrintState(new int[] { 1,2})+ " -- " + PrintState(new int[] { 1, 2, 3}) + " \n" + 
            "     \\ \n" +
            "       " + PrintState(new int[] { 2 }) + " -- " + PrintState(new int[] { 2,1 }) + " -- " + PrintState(new int[] { 2, 1, 3 }) + ""); 
   }
}

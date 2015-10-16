/**
 * Copyright 2015 IBM Corp. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * @author Dogukan Erenel (derenel@us.ibm.com)
*/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Widgets;

namespace IBM.Watson.Editor
{
    //[CustomPropertyDrawer(typeof(Widget.Input))]
    //public class WidgetInputDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        // Using BeginProperty / EndProperty on the parent property means that
    //        // prefab override logic works on the entire property.
    //        EditorGUI.BeginProperty(position, label, property);

    //        // Draw label
    //        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //        // Don't make child fields be indented
    //        var indent = EditorGUI.indentLevel;
    //        EditorGUI.indentLevel = 0;

    //        // Calculate rects
    //        var amountRect = new Rect(position.x, position.y, 30, position.height);
    //        var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
    //        var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

    //        // Draw fields - passs GUIContent.none to each so they are drawn without labels
    //        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
    //        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
    //        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

    //        // Set indent back to what it was
    //        EditorGUI.indentLevel = indent;

    //        EditorGUI.EndProperty();
    //    }
    //}

    /// <summary>
    /// Widget editor to use Widgets with drag/drop
    /// </summary>
    public class WidgetEditor: EditorWindow {

		private Texture m_WatsonIcon = null;

		private void OnEnable()
		{
			titleContent.text = "Watson Editor";
			m_WatsonIcon = (Texture2D)Resources.Load( EditorValues.WatsonIcon32, typeof(Texture2D) );
			if (m_WatsonIcon != null)
				titleContent.image = m_WatsonIcon;
			else
				Log.Error ("WidgetEditor", "WatsonIcon couldn't find");
		}

		
		[MenuItem("Window/Watson Widget editor")]
		static void ShowWidgetEditor() {
			WidgetEditor widgetEditor = WidgetEditor.GetWindow<WidgetEditor>();
			widgetEditor.Init();
		}

		
		Rect[] windows;
		Widget[] widgetsOnSceen = null;
		public void Init() {

			//widgetsOnSceen = GameObject.FindObjectsOfType<Widget> ();
			//if (widgetsOnSceen != null) {
			//	windows = new Rect[widgetsOnSceen.Length];
			//	for (int indexWidget = 0; indexWidget < widgetsOnSceen.Length; indexWidget++) {
			//		widgetsOnSceen[indexWidget].SetupWidgetIOForEditor();
			//		windows [indexWidget] = new Rect (100 + indexWidget * 300, 0, 100, 200);  
			//		//windows [indexWidget].min = new Vector2(10, 10);
			//		//windows [indexWidget].max = new Vector2(200, 200);
			//	}
			//} else {
			//	windows = null;
			//}
		}

		void OnInspectorUpdate(){
			//Debug.Log ("OnSceneGUI: " + Event.current.type);
			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("GetMouseButtonDown: " );
			}
		}

		
		void OnGUI() {
			//DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows

			BeginWindows();


//			if (windows != null) {
//				for (int i = 0; i < windows.Length; i++) {

//					//

//					GUI.BeginGroup(new Rect (windows[i].x - 100, windows[i].y, 300, 200));
//					windows[i] =  GUI.Window(i, windows[i], DrawNodeWindow, widgetsOnSceen[i].name.Replace("Widget", "")); 
//					//GUI.Box(new Rect(0, 0, 50, 20), "TEST");

//					//if (GUI.Button(new Rect(0, 25, 50, 20), "Hello World"))
//					//	Debug.Log("Got a click in window with color " + GUI.color);

////					for (int indexInput = 0; indexInput < widgetsOnSceen[i].inputList.Count; indexInput++) {
////						GUI.Box(new Rect(0, indexInput * 20 , 100, 20), widgetsOnSceen[i].inputList[indexInput].name);
////					}

//					int indexInput = 0;
//					foreach( KeyValuePair <IOType, System.Action<WatsonIO>> inputItem in widgetsOnSceen[i].inputList )
//					{
//						GUI.Box(new Rect(0, indexInput * 20 , 100, 20), inputItem.Key.ToFriendlyString());
//						indexInput ++;
//					}

//					int indexOutput = 0;
//					foreach( KeyValuePair <IOType, System.Action<WatsonIO>> outputItem in widgetsOnSceen[i].outputList )
//					{
//						indexOutput ++;
//						//Debug.Log(" widgetsOnSceen[i].outputList[indexOutput].Target.GetType().ToString() : " +  outputItem.Key.ToString());
//						GUI.Box(new Rect(200, 200 - (indexOutput) * 20 , 100, 20), outputItem.Key.ToFriendlyString());
//					}

////					for (int indexOutput = 0; indexOutput < widgetsOnSceen[i].outputList.Keys.Count; indexOutput++) {
////						Debug.Log(" widgetsOnSceen[i].outputList[indexOutput].Target.GetType().ToString() : " +  widgetsOnSceen[i].outputList.Keys[indexOutput]);
////						//GUI.Box(new Rect(200, 200 - (indexOutput + 1) * 20 , 100, 20), widgetsOnSceen[i].outputList.Keys[indexOutput]);
////					}

//					GUI.EndGroup();


//				}
//			}

			EndWindows();
			//window1 = GUI.Window(1, window1, DrawNodeWindow, "Node 1");   // Updates the Rect's when these are dragged
			//window2 = GUI.Window(2, window2, DrawNodeWindow, "Node 2");

			DrawNodeCurve (windows [0], windows [1]);
		}
		
		void DrawNodeWindow(int id) {
			GUI.DragWindow();
		
//			int indexInput = 0;
//			foreach( KeyValuePair <IOType, System.Action<WatsonIO>> inputItem in widgetsOnSceen[i]. )
//			{
//				GUI.Box(new Rect(0, indexInput * 20 , 100, 20), inputItem.Key.ToString());
//				indexInput ++;
//			}

			//if (GUI.Button(new Rect(-10, 20, 100, 20), "Hello World"))
			//	Debug.Log("Got a click in window with color " + GUI.color);

			//Debug.Log ("DrawNodeWindow: " + id + " -  widgetsOnSceen " + widgetsOnSceen.Length + " - widgetsOnSceen["+id+"].inputList.Count: " + widgetsOnSceen[id].inputList.Count);
//			for (int indexInput = 0; indexInput < widgetsOnSceen[id].inputList.co; indexInput++) {
//				//widgetsOnSceen[i].inputList[indexInput].
//				//Debug.Log("widgetsOnSceen["+id+"].inputList["+indexInput+"].name : " + widgetsOnSceen[id].inputList[indexInput].name);
//				GUILayout.Label(widgetsOnSceen[id].inputList[indexInput].name);
//			}
		}
		
		void DrawNodeCurve(Rect start, Rect end) {
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
			Vector3 startTan = startPos + Vector3.right * 50;
			Vector3 endTan = endPos + Vector3.left * 50;
			Color shadowCol = new Color(0, 0, 0, 0.06f);
			for (int i = 0; i < 3; i++) // Draw a shadow
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
		}
	}
}

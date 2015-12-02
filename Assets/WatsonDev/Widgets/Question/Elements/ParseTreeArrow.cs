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
*/

using UnityEngine;
using System.Collections;

namespace IBM.Watson.Widgets.Question
{
	public class ParseTreeArrow : MonoBehaviour {
		public RectTransform parentRectTransform;
		public RectTransform childRectTransform;

		void Update()
		{
			UpdateArrow();
		}

		//	test update
		public void UpdateArrow()
		{
			//	find two points
			Vector3 parentPoint = parentRectTransform.position - (new Vector3(0f, parentRectTransform.rect.height/2 + 50f, 0f) * 0.01f);
			Vector3 childPoint = childRectTransform.position + (new Vector3(0f, childRectTransform.rect.height/2, 0f) * 0.01f);
			
			Vector3 direction = childPoint - parentPoint;
			direction = parentRectTransform.TransformDirection(direction);
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - 90f;

			RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
			rectTransform.position = parentPoint;
			rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

			RectTransform arrowRectTransform = gameObject.GetComponent<RectTransform>();
			float dist = Vector3.Distance(parentPoint, childPoint);
			Vector2 tempSizedelta = arrowRectTransform.sizeDelta;
			tempSizedelta.x = dist * 45f;
			arrowRectTransform.sizeDelta = tempSizedelta;
			arrowRectTransform.SetParent(parentRectTransform, false);
		}
	}
}
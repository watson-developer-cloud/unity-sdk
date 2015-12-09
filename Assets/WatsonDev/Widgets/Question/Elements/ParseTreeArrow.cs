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
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
	public class ParseTreeArrow : MonoBehaviour {
		public RectTransform ParentRectTransform;
		public RectTransform ChildRectTransform;
		private Canvas FacetCanvas;

		void Update()
		{
			UpdateArrow();
		}

		//	test update
		private void UpdateArrow()
		{
			Vector3 localStartPoint = new Vector3(0f, -ParentRectTransform.rect.height/2 - 10f, 0f);
			Vector3 localEndPoint = GetPositionInCanvasSpace(ChildRectTransform) - GetPositionInCanvasSpace(ParentRectTransform) + new Vector3(0f, ChildRectTransform.rect.height/2, 0f);

			RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = localStartPoint;
		
			//	arrow direction
			Vector3 direction = (localEndPoint - localStartPoint).normalized;
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - 90f;
			rectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));

			//	length of arrow
			float dist = Vector3.Distance(localStartPoint, localEndPoint);
			Vector3 tempSizedelta = rectTransform.sizeDelta;
			tempSizedelta.x = dist;
			rectTransform.sizeDelta = tempSizedelta;
//			rectTransform.SetParent(ParentRectTransform, false);
		}

		private Vector3 GetPositionInCanvasSpace(RectTransform rectTransform)
		{
			Vector3 resultPoint = Vector3.left;
			RectTransform[] rectTransformArray = rectTransform.GetComponentsInParent<RectTransform>();

			foreach(RectTransform parentRectTransform in rectTransformArray)
			{
				resultPoint += parentRectTransform.localPosition;
			}
			return resultPoint;
		}
	}
}
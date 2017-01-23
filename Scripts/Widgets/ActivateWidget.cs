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

using IBM.Watson.DeveloperCloud.DataTypes;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Widgets
{
  /// <summary>
  /// Simple widget class for connecting a UI button to an activation input on a widget.
  /// </summary>
  public class ActivateWidget : Widget
  {
    #region Outputs
    [SerializeField]
    private Output m_ActivateOutput = new Output(typeof(BooleanData), true);
    #endregion

    #region Private Data
    [SerializeField]
    private bool m_SendValue = true;
    #endregion

    #region Widget interface
    /// <exclude />
    protected override string GetName()
    {
      return "Activate";
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Button event handler.
    /// </summary>
    public void OnButton()
    {
      m_ActivateOutput.SendData(new BooleanData(m_SendValue));
    }
    #endregion
  }
}

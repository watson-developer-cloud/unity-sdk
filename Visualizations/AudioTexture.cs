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
*/

using UnityEngine;
using System.Collections;

public class AudioTexture : MonoBehaviour {

    #region Private Data
    [SerializeField]
    private Renderer m_Target = null;
    [SerializeField]
    private AudioSource m_Source = null;
    [SerializeField]
    private Vector2 m_TexSize = new Vector2( 32.0f, 32.0f );

    private Texture2D m_AudioTexture = null;
    #endregion

    // Use this for initialization
    void Start () {
        m_AudioTexture = new Texture2D( (int)m_TexSize.x, (int)m_TexSize.y, TextureFormat.ARGB32, false );

        if ( m_Target != null )
        {
            m_Target.material.SetTexture( "Normal Map", m_AudioTexture );
        }
	}
	
	// Update is called once per frame
	void Update () {
        if ( m_Source != null )
        {
        }
	}
}

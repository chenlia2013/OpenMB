﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;
using Mogre_Procedural.MogreBites.Addons;

namespace AMOFGameEngine
{
    class SinbadState : AppState
    {
	    ParamsPanel		m_DetailsPanel;   		// sample details panel
	    bool						m_bQuit;
	    bool						m_CursorWasVisible;		// was cursor visible before dialog appeared
	    bool						m_DragLook;              // click and drag to free-look
        public SdkCameraMan	m_CameraMan;
	    SinbadCharacterController	m_Chara;
	    NameValuePairList		mInfo=new NameValuePairList();    // custom sample info
        public SinbadState()
        {
            m_bQuit = false;
            m_DetailsPanel = null;
            m_Camera = null;
            m_CameraMan = null;
            m_Chara = null;
        }

        public override void enter()
        {
            AdvancedMogreFramework.Singleton.m_Log.LogMessage("Entering SinbadState...");
            AdvancedMogreFramework.LastStateName = "SinbadState";
            m_SceneMgr = AdvancedMogreFramework.Singleton.m_Root.CreateSceneManager(SceneType.ST_GENERIC, "SinbadSceneMgr");

            m_Camera = m_SceneMgr.CreateCamera("MainCamera");
	        AdvancedMogreFramework.Singleton.m_Viewport.Camera=m_Camera;
            m_Camera.AspectRatio = (float)AdvancedMogreFramework.Singleton.m_Viewport.ActualWidth / (float)AdvancedMogreFramework.Singleton.m_Viewport.ActualHeight;
	        m_Camera.NearClipDistance=5;

	        m_CameraMan = new SdkCameraMan(m_Camera);

            AdvancedMogreFramework.Singleton.m_Mouse.MouseMoved += new MouseListener.MouseMovedHandler(mouseMoved);
            AdvancedMogreFramework.Singleton.m_Mouse.MousePressed += new MouseListener.MousePressedHandler(mousePressed);
            AdvancedMogreFramework.Singleton.m_Mouse.MouseReleased += new MouseListener.MouseReleasedHandler(mouseReleased);
            AdvancedMogreFramework.Singleton.m_Keyboard.KeyPressed += new KeyListener.KeyPressedHandler(keyPressed);
            AdvancedMogreFramework.Singleton.m_Keyboard.KeyReleased += new KeyListener.KeyReleasedHandler(keyReleased);

            AdvancedMogreFramework.Singleton.m_Root.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(FrameRenderingQueued);

            buildGUI();
 
            createScene();
        }
        public void createScene()
        {
            // set background and some fog
	        AdvancedMogreFramework.Singleton.m_Viewport.BackgroundColour=new ColourValue(1.0f, 1.0f, 0.8f);
	        m_SceneMgr.SetFog(FogMode.FOG_LINEAR, new ColourValue(1.0f, 1.0f, 0.8f), 0, 15, 100);

	        // set shadow properties
	        m_SceneMgr.ShadowTechnique=ShadowTechnique.SHADOWTYPE_TEXTURE_MODULATIVE;
	        m_SceneMgr.ShadowColour=new ColourValue(0.5f, 0.5f, 0.5f);
	        m_SceneMgr.SetShadowTextureSize(1024);
	        m_SceneMgr.ShadowTextureCount=1;

	        // disable default camera control so the character can do its own
	        m_CameraMan.setStyle(CameraStyle.CS_MANUAL);
	        // use a small amount of ambient lighting
	        m_SceneMgr.AmbientLight=new ColourValue(0.3f, 0.3f, 0.3f);

	        // add a bright light above the scene
	        Light light = m_SceneMgr.CreateLight();
	        light.Type=(Light.LightTypes. LT_POINT);
	        light.Position=new Mogre.Vector3(-10, 40, 20);
	        light.SpecularColour=ColourValue.White;

	        // create a floor mesh resource
	        MeshManager.Singleton.CreatePlane("floor", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
		        new Plane(Mogre.Vector3.UNIT_Y, 0), 100, 100, 10, 10, true, 1, 10, 10, Mogre.Vector3.UNIT_Z);

	        // create a floor entity, give it a material, and place it at the origin
            Entity floor = m_SceneMgr.CreateEntity("Floor", "floor");
            floor.SetMaterialName("Examples/Rockwall");
	        floor.CastShadows=(false);
            m_SceneMgr.RootSceneNode.AttachObject(floor);

	        // create our character controller
	        m_Chara = new SinbadCharacterController(m_Camera);

	        AdvancedMogreFramework.Singleton.m_TrayMgr.toggleAdvancedFrameStats();

	        StringVector items=new StringVector();
	        items.Insert(items.Count,"Help");
	        ParamsPanel help = AdvancedMogreFramework.Singleton.m_TrayMgr.createParamsPanel(TrayLocation. TL_TOPLEFT, "HelpMessage", 100, items);
	        help.setParamValue("Help", "H / F1");
        }
        public override void exit()
        {
            AdvancedMogreFramework.Singleton.m_Log.LogMessage("Leaving SinbadState...");
 
            m_SceneMgr.DestroyCamera(m_Camera);
	        if (m_CameraMan!=null) m_CameraMan=null;
	
            if(m_SceneMgr!=null)
                AdvancedMogreFramework.Singleton.m_Root.DestroySceneManager(m_SceneMgr);
        }
        public override bool pause()
        {
            AdvancedMogreFramework.Singleton.m_Log.LogMessage("Pausing SinbadState...");
            return true;
        }
        public override void resume()
        {
            AdvancedMogreFramework.Singleton.m_Log.LogMessage("Resuming SinbadState...");
 
            buildGUI();

            AdvancedMogreFramework.Singleton.m_Viewport.Camera=m_Camera;
            m_bQuit = false;
        }

        void buildGUI()
        {
            AdvancedMogreFramework.Singleton.m_TrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
	        AdvancedMogreFramework.Singleton.m_TrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
	        AdvancedMogreFramework.Singleton.m_TrayMgr.createLabel(TrayLocation.TL_TOP, "GameLbl", "Game mode", 250);
	        AdvancedMogreFramework.Singleton.m_TrayMgr.showCursor();


            // create a params panel for displaying sample details
	        StringVector items=new StringVector();
	        items.Insert(items.Count,"cam.pX");
	        items.Insert(items.Count,"cam.pY");
	        items.Insert(items.Count,"cam.pZ");
	        items.Insert(items.Count,"");
	        items.Insert(items.Count,"cam.oW");
	        items.Insert(items.Count,"cam.oX");
	        items.Insert(items.Count,"cam.oY");
	        items.Insert(items.Count,"cam.oZ");
	        items.Insert(items.Count,"");
	        items.Insert(items.Count,"Filtering");
	        items.Insert(items.Count,"Poly Mode");

            m_DetailsPanel = AdvancedMogreFramework.Singleton.m_TrayMgr.createParamsPanel(TrayLocation.TL_NONE, "DetailsPanel", 200, items);
	        m_DetailsPanel.hide();

	        m_DetailsPanel.setParamValue(9, "Bilinear");
	        m_DetailsPanel.setParamValue(10, "Solid");
        }

        public bool keyPressed(KeyEvent evt)
        {
            if (!AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible()) m_Chara.injectKeyDown(evt);
	        if(AdvancedMogreFramework.Singleton.m_Keyboard.IsKeyDown(KeyCode.KC_ESCAPE))
            {
                pushAppState(findByName("PauseState"));
                return true;
            }

	        if (evt.key == KeyCode.KC_H || evt.key == KeyCode.KC_F1)   // toggle visibility of help dialog
			{
				if (!AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible() && mInfo["Help"] != "") AdvancedMogreFramework.Singleton.m_TrayMgr.showOkDialog("Help", mInfo["Help"]);
				else AdvancedMogreFramework.Singleton.m_TrayMgr.closeDialog();
			}

			if (AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible()) return true;   // don't process any more keys if dialog is up

			if (evt.key == KeyCode.KC_F)   // toggle visibility of advanced frame stats
			{
				AdvancedMogreFramework.Singleton.m_TrayMgr.toggleAdvancedFrameStats();
			}
			else if (evt.key == KeyCode.KC_G)   // toggle visibility of even rarer debugging details
			{
				if (m_DetailsPanel.getTrayLocation() == TrayLocation.TL_NONE)
				{
					AdvancedMogreFramework.Singleton.m_TrayMgr.moveWidgetToTray(m_DetailsPanel, TrayLocation.TL_TOPRIGHT, 0);
					m_DetailsPanel.show();
				}
				else
				{
					AdvancedMogreFramework.Singleton.m_TrayMgr.removeWidgetFromTray(m_DetailsPanel);
					m_DetailsPanel.hide();
				}
			}
			else if (evt.key == KeyCode.KC_T)   // cycle polygon rendering mode
			{
				String newVal;
				TextureFilterOptions tfo;
				uint aniso;

                switch (Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(m_DetailsPanel.getParamValue(9))))
				{
				case "B":
					newVal = "Trilinear";
					tfo = TextureFilterOptions.TFO_TRILINEAR;
					aniso = 1;
					break;
				case "T":
					newVal = "Anisotropic";
					tfo = TextureFilterOptions.TFO_ANISOTROPIC;
					aniso = 8;
					break;
				case "A":
					newVal = "None";
					tfo = TextureFilterOptions.TFO_NONE;
					aniso = 1;
					break;
				default:
					newVal = "Bilinear";
					tfo = TextureFilterOptions.TFO_BILINEAR;
					aniso = 1;
                    break;
				}

				MaterialManager.Singleton.SetDefaultTextureFiltering(tfo);
				MaterialManager.Singleton.DefaultAnisotropy=aniso;
				m_DetailsPanel.setParamValue(9, newVal);
			}
			else if (evt.key == KeyCode.KC_R)   // cycle polygon rendering mode
			{
				String newVal;
				PolygonMode pm;

				switch (m_Camera.PolygonMode)
				{
				case PolygonMode.PM_SOLID:
					newVal = "Wireframe";
					pm = PolygonMode.PM_WIREFRAME;
					break;
				case PolygonMode.PM_WIREFRAME:
					newVal = "Points";
					pm = PolygonMode.PM_POINTS;
					break;
				default:
					newVal = "Solid";
					pm = PolygonMode.PM_SOLID;
                    break;
				}

				m_Camera.PolygonMode=pm;
				m_DetailsPanel.setParamValue(10, newVal);
			}
			else if(evt.key == KeyCode.KC_F5)   // refresh all textures
			{
				TextureManager.Singleton.ReloadAll();
			}
			else if (evt.key == KeyCode.KC_SYSRQ)   // take a screenshot
			{
                AdvancedMogreFramework.Singleton.m_RenderWnd.WriteContentsToTimestampedFile("screenshot", ".png");
			}

			m_CameraMan.injectKeyDown(evt);
			return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            m_Chara.injectKeyUp(keyEventRef);
	        m_CameraMan.injectKeyUp(keyEventRef);
            AdvancedMogreFramework.Singleton.keyPressed(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent arg)
        {
            if (!AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible()) m_Chara.injectMouseMove(arg);
            if (AdvancedMogreFramework.Singleton.m_TrayMgr.injectMouseMove(arg)) return true;
	        m_CameraMan.injectMouseMove(arg);
  
            return true;
        }
        public bool mousePressed(MouseEvent arg, MouseButtonID id)
        {
            // relay input events to character controller
	        if (!AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible()) m_Chara.injectMouseDown(arg, id);
	        if (AdvancedMogreFramework.Singleton.m_TrayMgr.injectMouseDown(arg, id)) return true;
            
	        if (m_DragLook && id == MouseButtonID.MB_Left)
	        {
		        m_CameraMan.setStyle(CameraStyle.CS_FREELOOK);
                AdvancedMogreFramework.Singleton.m_TrayMgr.hideCursor();
	        }

            m_CameraMan.injectMouseDown(arg, id);
            return true;
        }
        public bool mouseReleased(MouseEvent arg, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Singleton.m_TrayMgr.injectMouseUp(arg, id)) return true;
            
	        if (m_DragLook && id == MouseButtonID.MB_Left)
	        {
		        m_CameraMan.setStyle(CameraStyle.CS_MANUAL);
                AdvancedMogreFramework.Singleton.m_TrayMgr.showCursor();
	        }

            m_CameraMan.injectMouseUp(arg, id);

	        return true;
        }

        public override void itemSelected(SelectMenu menu)
        {
            switch(menu.getSelectionIndex())
            {
            case 0:
                m_Camera.PolygonMode=(PolygonMode.PM_SOLID);break;
            case 1:
                m_Camera.PolygonMode=(PolygonMode.PM_WIREFRAME);break;
            case 2:
                m_Camera.PolygonMode=(PolygonMode.PM_POINTS);break;
    }
        }

        public override void update(double timeSinceLastFrame)
        {
            m_FrameEvent.timeSinceLastFrame = (int)timeSinceLastFrame;

	        //m_pChara.addTime((float)timeSinceLastFrame);

            AdvancedMogreFramework.Singleton.m_TrayMgr.frameRenderingQueued(m_FrameEvent);

            if(m_bQuit == true)
            {
                popAppState();
                return;
            }
 
	        if (!AdvancedMogreFramework.Singleton.m_TrayMgr.isDialogVisible())
	        {
		        m_CameraMan.frameRenderingQueued(m_FrameEvent);   // if dialog isn't up, then update the camera

		        if (m_DetailsPanel.isVisible())   // if details panel is visible, then update its contents
		        {
			        m_DetailsPanel.setParamValue(0, StringConverter.ToString(m_Camera.DerivedPosition.x));
                    m_DetailsPanel.setParamValue(1, StringConverter.ToString(m_Camera.DerivedPosition.y));
                    m_DetailsPanel.setParamValue(2, StringConverter.ToString(m_Camera.DerivedPosition.z));
                    m_DetailsPanel.setParamValue(4, StringConverter.ToString(m_Camera.DerivedOrientation.w));
                    m_DetailsPanel.setParamValue(5, StringConverter.ToString(m_Camera.DerivedOrientation.x));
                    m_DetailsPanel.setParamValue(6, StringConverter.ToString(m_Camera.DerivedOrientation.y));
                    m_DetailsPanel.setParamValue(7, StringConverter.ToString(m_Camera.DerivedOrientation.z));
		        }	
	        }
        }
        public bool FrameRenderingQueued(FrameEvent evt)
        {
            // let character update animations and camera
            m_Chara.addTime(evt.timeSinceLastFrame);
            return true;
        }
}
}
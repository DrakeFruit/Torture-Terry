using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Editor;
using Editor.Assets;
using Editor.Inspectors;
using Editor.MeshEditor;
using Editor.ShaderGraph;
using Editor.TextureEditor;
using Sandbox;
using Sandbox.ModelEditor;
using Sandbox.ModelEditor.Nodes;
[Dock( "Editor", "Opt Icon Generator", "dirty_lens" )]
public class OIG : Widget
{
	[Property]
	[Category( "GAME" )]
	public Model _Model { get; set; }

	public class IconDefinitions
	{
		public Rotation Angle { get; set; } = Rotation.From( 45, 45, 0 );
		public int Frames { get; set; } = 60;
		public float RotationSpeed { get; set; } = 1.0f;
		public int Width { get; set; } = 512;
		public int Height { get; set; } = 512;
		public float RotationDuration => 360 / RotationSpeed;
	}

	[Property]
	[Category( "GAME" )]
	public IconDefinitions IconDefinition { get; set; } = new IconDefinitions();


	[Property]
	[Category( "GAME" )]
	public string OutputPath { get; set; } = Editor.FileSystem.Content.GetFullPath( "" ) + "\\OIG\\";

	public string OutputPathLocal { get; set; } = "\\OIG\\";



	Texture _Texture;

	Editor.TextureEditor.Preview PREVIEW;

	public OIG( Widget parent ) : base( parent, false )
	{
		Layout = Layout.Column();
		Layout.Margin = 4;
		Layout.Spacing = 4;

		SetStyles( "background-color: #303445; color: white; font-weight: 600;" );

		var controlSheet = new ControlSheet();
		this.GetSerialized().TryGetProperty( "_Model", out SerializedProperty prop );
		controlSheet.AddRow( prop );

		this.GetSerialized().TryGetProperty( "IconDefinition", out SerializedProperty prop2 );
		controlSheet.AddObject( IconDefinition.GetSerialized() );
		Layout.Add( controlSheet );

		//Layout.Add( new Label( "Select a Model:", this ) );

		// Botão para Thumbnail
		var btnThumbnail = Layout.Add( new Button( "Generate Icon", this ) );
		btnThumbnail.Clicked += () =>
		{
			if ( _Model != null )
				GenerateThumbnail( _Model );
			else
				Log.Warning( "Select  a model!" );
		}; 

		//PREVIEW
		Layout.Add( new Label( "Preview:", this ) );

		var img = new Editor.TextureEditor.Preview( this );
		img.Texture = _Texture;//Texture.Load( "output/sm_prop_money_stack_clean_01/sm_prop_money_stack_clean_01_thumbnail.png" );
		img.Size = new Vector2( 512, 512 );
		PREVIEW = Layout.Add( img, 100 );


	}

	private void GenerateThumbnail( Model model )
	{
		string modelName = Path.GetFileNameWithoutExtension( model.Name );
		string modelFolder = Path.Combine( OutputPath, modelName );

		Directory.CreateDirectory( modelFolder );

		var sceneWorld = new SceneWorld();
		var sceneModel = new SceneModel( sceneWorld, model, new() );
		var sceneLight = new SceneDirectionalLight( sceneWorld, IconDefinition.Angle, Color.White );
		sceneLight.Rotation = IconDefinition.Angle; // Alinha a luz com a câmera
		var sceneCamera = new SceneCamera
		{
			Ortho = true,
			World = sceneWorld,
			Rotation = IconDefinition.Angle
		};

		var bounds = sceneModel.Bounds;

		float maxDimension = MathF.Max( bounds.Size.x, MathF.Max( bounds.Size.y, bounds.Size.z ) );
		sceneCamera.OrthoHeight = maxDimension * 1.4f;
		sceneCamera.Position = IconDefinition.Angle.Backward * (maxDimension * 2.5f) + bounds.Center;

		var texture = Texture.CreateRenderTarget().WithSize( IconDefinition.Width, IconDefinition.Height ).Create();
		Graphics.RenderToTexture( sceneCamera, texture );

		var pixelMap = Pixmap.FromTexture( texture );
		//Log Info MyModel.Name but split / and remove the extension	 
		string filePath = Path.Combine( modelFolder, $"{modelName}_thumbnail.png" );
		pixelMap.SavePng( filePath );




		// Limpeza dos recursos
		texture.Dispose();
		sceneModel.Delete();
		sceneLight.Delete();
		sceneWorld.Delete();

		Log.Info( $"Icon Generated: {filePath}" );
		string localPath = OutputPathLocal + modelName + "\\" + $"{modelName}_thumbnail.png";	
		_Texture = Texture.Load( localPath );
		PREVIEW.Texture = _Texture;

	}

}

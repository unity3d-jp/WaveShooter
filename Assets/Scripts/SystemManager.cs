/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */
#if UNITY_PS4 || UNITY_PSP2 || UNITY_SWITCH
# define UTJ_MULTI_THREADED
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UTJ {

public class SystemManager : MonoBehaviour {

	// singleton
	static SystemManager instance_;
	public static SystemManager Instance { get { return instance_ ?? (instance_ = GameObject.Find("system_manager").GetComponent<SystemManager>()); } }

	public bool water_surface_distortion_ = true;
	public bool water_surface_line_render_ = false;
	public GameObject player_prefab_;
	public GameObject enemy_zako_prefab_;
	public GameObject dragon_head_prefab_;
	public GameObject dragon_body_prefab_;
	public GameObject dragon_tail_prefab_;
	public Material final_material_;
	public Material spark_material_;
	public Material beam_material_;
	public Material beam2_material_;
	public Material water_surface_input_material_;
	public Material water_splash_material_;
	public Material explosion_material_;
	public Material hahen_material_;
	public Material shield_material_;
	public Material debris_material_;
	public Material dust_material_;
	public Sprite[] sprites_;
	public Material sprite_material_;
	public Material sight_material_;
	public Font font_;
	public Material font_material_;
	public GameObject canvas_;

	public GameObject camera_holder_;
	private Camera camera_;
	public GameObject camera_final_holder_;
	private Camera camera_final_;
	private RenderTexture render_texture_;
	public Matrix4x4 ProjectionMatrix { get; set; }

	private const int DefaultFps = 60;
	const float RENDER_FPS = 60f;
	const float RENDER_DT = 1f/RENDER_FPS;
	public System.Diagnostics.Stopwatch stopwatch_;
	private Thread update_thread_;
	private int rendering_front_;
	public int getRenderingFront() { return rendering_front_; }
	private DrawBuffer[] draw_buffer_;
#if UTJ_MULTI_THREADED
	private System.Threading.ManualResetEvent manual_reset_event_;
	private long update_sync_frame_;
#endif
	private float dt_;
	public void setFPS(int fps)	{
		dt_ = 1.0f / (float)fps;
	}

	private CameraBase my_camera_;
	private CameraBase spectator_camera_;

	private long update_frame_;
	private long render_frame_;
	private long render_sync_frame_;
	private float update_time_;
	private bool pause_;
	// private long update_tick_;
	// private long render_update_tick_;

	private const int ENEMY_ZAKO_MAX = 32;
	private const int DRAGON_HEAD_MAX = 1;
	private const int DRAGON_BODY_MAX = 8;
	private const int DRAGON_TAIL_MAX = 1;

	private MuscleMotionRenderer muscle_motion_renderer_player_;
	private GameObject[] enemy_zako_pool_;
	private GameObject[] dragon_head_pool_;
	private GameObject[] dragon_body_pool_;
	private GameObject[] dragon_tail_pool_;

	// audio
	public const int AUDIO_CHANNEL_MAX = 4;

	private const int AUDIOSOURCE_BULLET_MAX = 4;
	private AudioSource[] audio_sources_bullet_;
	int audio_source_bullet_index_;
	public AudioClip se_bullet_;

	private const int AUDIOSOURCE_EXPLOSION_MAX = 4;
	private AudioSource[] audio_sources_explosion_;
	int audio_source_explosion_index_;
	public AudioClip se_explosion_;

	private const int AUDIOSOURCE_LASER_MAX = 4;
	private AudioSource[] audio_sources_laser_;
	int audio_source_laser_index_;
	public AudioClip se_laser_;

	private const int AUDIOSOURCE_SHIELD_MAX = 4;
	private AudioSource[] audio_sources_shield_;
	int audio_source_shield_index_;
	public AudioClip se_shield_;

	private AudioSource audio_sources_bgm_;
	public AudioClip bgm01_;
	private bool is_bgm_playing_;

	private bool spectator_mode_;
	private bool initialized_ = false;
	private bool auto_ = true;

	IEnumerator Start()
	{
		yield return SystemManager.Instance.initialize();
	}

    void OnApplicationQuit()
	{
		UnityPluginIF.Unload();
    }

	private void set_camera(bool spectator_mode)
	{
		my_camera_.active_ = !spectator_mode;
		spectator_camera_.active_ = spectator_mode;
	}

	private IEnumerator initialize()
	{
		camera_final_ = camera_final_holder_.GetComponent<Camera>();
		camera_final_.enabled = false;
		UnityPluginIF.Load("UnityPlugin");

		Application.targetFrameRate = (int)RENDER_FPS;
		DontDestroyOnLoad(gameObject);

		stopwatch_ = new System.Diagnostics.Stopwatch();
		stopwatch_.Start();
		rendering_front_ = 0;

#if UTJ_MULTI_THREADED
		manual_reset_event_ = new System.Threading.ManualResetEvent(false);
		update_sync_frame_ = 0;
#endif
		setFPS(DefaultFps);
		update_frame_ = 0;
		update_time_ = 0f;
		render_frame_ = 0;
		render_sync_frame_ = 0;
		pause_ = false;
		spectator_mode_ = auto_;
		canvas_.SetActive(false);

		camera_ = camera_holder_.GetComponent<Camera>();
		ProjectionMatrix = camera_.projectionMatrix;

		BoxingPool.init();
		InputManager.Instance.init();
		Controller.Instance.init(auto_);
		TaskManager.Instance.init();
		MyCollider.createPool();
		Bullet.createPool();
		EnemyBullet.createPool();
		EnemyLaser.createPool();
		Enemy.createPool();
		WaterSurface.Instance.init(water_surface_input_material_, water_surface_distortion_, water_surface_line_render_);
		WaterSurfaceRenderer.Instance.init(camera_.transform);
		Spark.Instance.init(spark_material_);
		Beam.Instance.init(beam_material_);
		BeamRenderer.Instance.init(Beam.Instance);
		Beam2.Instance.init(beam2_material_);
		Beam2Renderer.Instance.init(Beam2.Instance);
		WaterSplash.Instance.init(water_splash_material_, WaterSurfaceRenderer.Instance.getReflectionTexture());
		WaterSplashRenderer.Instance.init(WaterSplash.Instance);
		AuraEffect.Instance.init();
		Explosion.Instance.init(explosion_material_);
		ExplosionRenderer.Instance.init(Explosion.Instance);
		Hahen.Instance.init(hahen_material_);
		HahenRenderer.Instance.init(Hahen.Instance);
		Shield.Instance.init(shield_material_);
		ShieldRenderer.Instance.init(Shield.Instance);
		Debris.Instance.init(debris_material_);
		Dust.Instance.init(dust_material_);
		LightEnvironmentController.Instance.init();
		Sight.Instance.init(sight_material_);
		SightRenderer.Instance.init(Sight.Instance);
		MySprite.Instance.init(sprites_, sprite_material_);
		MySpriteRenderer.Instance.init(camera_);
		MyFont.Instance.init(font_, font_material_);
		MyFontRenderer.Instance.init();
		GaugeJump.Create();

		PerformanceMeter.Instance.init();

		draw_buffer_ = new DrawBuffer[2];
		for (int i = 0; i < 2; ++i) {
			draw_buffer_[i].init();
		}

		yield return Player.Instance.initialize();
		my_camera_ = MyCamera.create();
		spectator_camera_ = SpectatorCamera.create();
		set_camera(spectator_mode_);

		if (player_prefab_ != null) {
			GameObject go = Instantiate(player_prefab_);
			// player_renderer_ = go.GetComponent<PlayerRenderer>();
			muscle_motion_renderer_player_ = go.GetComponent<MuscleMotionRenderer>();
			muscle_motion_renderer_player_.init();
		}
		if (enemy_zako_prefab_ != null) {
			enemy_zako_pool_ = new GameObject[ENEMY_ZAKO_MAX];
			for (var i = 0; i < ENEMY_ZAKO_MAX; ++i) {
				enemy_zako_pool_[i] = Instantiate(enemy_zako_prefab_) as GameObject;
				enemy_zako_pool_[i].SetActive(false);
			}
		}
		if (dragon_head_prefab_ != null) {
			dragon_head_pool_ = new GameObject[DRAGON_HEAD_MAX];
			for (var i = 0; i < DRAGON_HEAD_MAX; ++i) {
				dragon_head_pool_[i] = Instantiate(dragon_head_prefab_) as GameObject;
				dragon_head_pool_[i].SetActive(false);
			}
		}
		if (dragon_body_prefab_ != null) {
			dragon_body_pool_ = new GameObject[DRAGON_BODY_MAX];
			for (var i = 0; i < DRAGON_BODY_MAX; ++i) {
				dragon_body_pool_[i] = Instantiate(dragon_body_prefab_) as GameObject;
				dragon_body_pool_[i].SetActive(false);
			}
		}
		if (dragon_tail_prefab_ != null) {
			dragon_tail_pool_ = new GameObject[DRAGON_TAIL_MAX];
			for (var i = 0; i < DRAGON_TAIL_MAX; ++i) {
				dragon_tail_pool_[i] = Instantiate(dragon_tail_prefab_) as GameObject;
				dragon_tail_pool_[i].SetActive(false);
			}
		}

		// audio
		audio_sources_bullet_ = new AudioSource[AUDIOSOURCE_BULLET_MAX];
		for (var i = 0; i < AUDIOSOURCE_BULLET_MAX; ++i) {
			audio_sources_bullet_[i] = gameObject.AddComponent<AudioSource>();
			audio_sources_bullet_[i].clip = se_bullet_;
			audio_sources_bullet_[i].volume = 0.04f;
		}
		audio_source_bullet_index_ = 0;
		audio_sources_explosion_ = new AudioSource[AUDIOSOURCE_EXPLOSION_MAX];
		for (var i = 0; i < AUDIOSOURCE_EXPLOSION_MAX; ++i) {
			audio_sources_explosion_[i] = gameObject.AddComponent<AudioSource>();
			audio_sources_explosion_[i].clip = se_explosion_;
			audio_sources_explosion_[i].volume = 0.025f;
		}
		audio_source_explosion_index_ = 0;
		audio_sources_laser_ = new AudioSource[AUDIOSOURCE_LASER_MAX];
		for (var i = 0; i < AUDIOSOURCE_LASER_MAX; ++i) {
			audio_sources_laser_[i] = gameObject.AddComponent<AudioSource>();
			audio_sources_laser_[i].clip = se_laser_;
			audio_sources_laser_[i].volume = 0.025f;
		}
		audio_source_laser_index_ = 0;
		audio_sources_shield_ = new AudioSource[AUDIOSOURCE_SHIELD_MAX];
		for (var i = 0; i < AUDIOSOURCE_SHIELD_MAX; ++i) {
			audio_sources_shield_[i] = gameObject.AddComponent<AudioSource>();
			audio_sources_shield_[i].clip = se_shield_;
			audio_sources_shield_[i].volume = 0.020f;
		}
		audio_source_shield_index_ = 0;
		audio_sources_shield_ = new AudioSource[AUDIOSOURCE_SHIELD_MAX];
		for (var i = 0; i < AUDIOSOURCE_SHIELD_MAX; ++i) {
			audio_sources_shield_[i] = gameObject.AddComponent<AudioSource>();
			audio_sources_shield_[i].clip = se_shield_;
			audio_sources_shield_[i].volume = 0.025f;
		}
		audio_source_shield_index_ = 0;
		audio_sources_bgm_ = gameObject.AddComponent<AudioSource>();
		audio_sources_bgm_.clip = bgm01_;
		audio_sources_bgm_.volume = 0.05f;
		audio_sources_bgm_.loop = true;
		is_bgm_playing_ = false;
	
		GameManager.Instance.init();

#if UTJ_MULTI_THREADED
		update_thread_ = new Thread(thread_entry);
		update_thread_.Priority = System.Threading.ThreadPriority.Highest;
		update_thread_.Start();
#endif

#if UNITY_PS4 || UNITY_EDITOR
		int rw = 1920;
		int rh = 1080;
#elif UNITY_PSP2
		int rw = 480;
		int rh = 272;
#else
		int rw = 568;
		int rh = 320;
#endif
		render_texture_ = new RenderTexture(rw, rh, 24 /* depth */, RenderTextureFormat.ARGB32);
		render_texture_.Create();
		camera_.targetTexture = render_texture_;
		final_material_.mainTexture = render_texture_;

		initialized_ = true;
		camera_final_.enabled = true;
	}

	void OnDestroy()
	{
		if (update_thread_ != null) {
			update_thread_.Abort();
		}
	}

	private int get_front()
	{
#if UTJ_MULTI_THREADED
		int updating_front = 1 - rendering_front_;		// flip
#else
		int updating_front = rendering_front_;			// don't flip
#endif
		return updating_front;
	}

	private void main_loop()
	{
		PerformanceMeter.Instance.beginUpdate();
		int updating_front = get_front();

		// fetch
		Controller.Instance.fetch(updating_front, update_time_);

		var controller = Controller.Instance.getLatest();
		if (!pause_ && controller.isPauseButtonDown()) {
			pause_ = true;
		}
#if UNITY_EDITOR
		else if (pause_ && controller.isPauseButtonDown()) {
			pause_ = false;
		}
#endif

 		// update
		if (!pause_) {
			int loop_num = 1;
			if (PerformanceMeter.Instance.wasSlowLoop()) {
				loop_num = 2;
			}
			for (var loop = 0; loop < loop_num; ++loop) {
				GameManager.Instance.update(dt_, update_time_);
				TaskManager.Instance.update(dt_, update_time_);
				MyCollider.calculate();
				WaterSplash.Instance.update(update_time_);
				++update_frame_;
				update_time_ += dt_;
			}
		}
		PerformanceMeter.Instance.endUpdate();

		PerformanceMeter.Instance.beginRenderUpdate();
		CameraBase current_camera = spectator_mode_ ? spectator_camera_ : my_camera_;
		// begin
		MySprite.Instance.begin();
		MyFont.Instance.begin();
		Spark.Instance.begin();
		Beam.Instance.begin(updating_front);
		Beam2.Instance.begin(updating_front);
		Explosion.Instance.begin();
		Hahen.Instance.begin();
		Shield.Instance.begin();
		Sight.Instance.begin(updating_front);

		// renderUpdate
 		draw_buffer_[updating_front].beginRender();
		WaterSurface.Instance.renderUpdate(updating_front);
		WaterSplash.Instance.renderUpdate(updating_front);
		TaskManager.Instance.renderUpdate(updating_front,
										  current_camera,
										  ref draw_buffer_[updating_front]);
		draw_buffer_[updating_front].endRender();
		
		// performance meter
#if UTJ_MULTI_THREADED
		bool multi_threading = true;
#else
		bool multi_threading = false;
#endif
		PerformanceMeter.Instance.drawMeters(updating_front, multi_threading);

		// end
		Sight.Instance.end(updating_front, current_camera);
		Shield.Instance.end(updating_front);
		Hahen.Instance.end(updating_front);
		Explosion.Instance.end(updating_front);
		Beam2.Instance.end();
		Beam.Instance.end();
		Spark.Instance.end(updating_front);
		MyFont.Instance.end(updating_front);
		MySprite.Instance.end(updating_front);

		PerformanceMeter.Instance.endRenderUpdate();
	}

#if UTJ_MULTI_THREADED
	private void thread_entry()
	{
		for (;;) {
			try {
				main_loop();
				// Debug.Assert(update_sync_frame_ >= render_sync_frame_);
				while (update_sync_frame_ >= render_sync_frame_) {
					manual_reset_event_.WaitOne();
					manual_reset_event_.Reset();
				}
				++update_sync_frame_;

			} catch (System.Exception e) {
				Debug.Log(e);
			}
		}
	}
#endif

	public float realtimeSinceStartup { get { return ((float)stopwatch_.ElapsedTicks) /  (float)System.Diagnostics.Stopwatch.Frequency; } }

	public void registSound(DrawBuffer.SE se)
	{
		int front = get_front();
		draw_buffer_[front].registSound(se);
	}

	public void registBgm(DrawBuffer.BGM bgm)
	{
		int front = get_front();
		draw_buffer_[front].registBgm(bgm);
	}

	/*
	 * 以下は MailThread
	 */
	// オブジェクト描画(SetActive)
	private void render(ref DrawBuffer draw_buffer)
	{
		// camera
		camera_.transform.position = draw_buffer.camera_transform_.position_;
		camera_.transform.rotation = draw_buffer.camera_transform_.rotation_;
		camera_.enabled = true;

		int enemy_zako_idx = 0;
		int dragon_head_idx = 0;
		int dragon_body_idx = 0;
		int dragon_tail_idx = 0;
		for (var i = 0; i < draw_buffer.object_num_; ++i) {
			switch (draw_buffer.object_buffer_[i].type_) {
				case DrawBuffer.Type.None:
					Debug.Assert(false);
					break;
				case DrawBuffer.Type.Empty:
					break;
				case DrawBuffer.Type.MuscleMotionPlayer:
					muscle_motion_renderer_player_.render(ref draw_buffer.object_buffer_[i]);
					break;
				case DrawBuffer.Type.Zako:
					enemy_zako_pool_[enemy_zako_idx].SetActive(true);
					enemy_zako_pool_[enemy_zako_idx].transform.localPosition = draw_buffer.object_buffer_[i].transform_.position_;
					enemy_zako_pool_[enemy_zako_idx].transform.localRotation = draw_buffer.object_buffer_[i].transform_.rotation_;
					++enemy_zako_idx;
					break;
				case DrawBuffer.Type.DragonHead:
					dragon_head_pool_[dragon_head_idx].SetActive(true);
					dragon_head_pool_[dragon_head_idx].transform.localPosition = draw_buffer.object_buffer_[i].transform_.position_;
					dragon_head_pool_[dragon_head_idx].transform.localRotation = draw_buffer.object_buffer_[i].transform_.rotation_;
					++dragon_head_idx;
					break;
				case DrawBuffer.Type.DragonBody:
					dragon_body_pool_[dragon_body_idx].SetActive(true);
					dragon_body_pool_[dragon_body_idx].transform.localPosition = draw_buffer.object_buffer_[i].transform_.position_;
					dragon_body_pool_[dragon_body_idx].transform.localRotation = draw_buffer.object_buffer_[i].transform_.rotation_;
					++dragon_body_idx;
					break;
				case DrawBuffer.Type.DragonTail:
					dragon_tail_pool_[dragon_tail_idx].SetActive(true);
					dragon_tail_pool_[dragon_tail_idx].transform.localPosition = draw_buffer.object_buffer_[i].transform_.position_;
					dragon_tail_pool_[dragon_tail_idx].transform.localRotation = draw_buffer.object_buffer_[i].transform_.rotation_;
					++dragon_tail_idx;
					break;
			}
		}
		for (var i = enemy_zako_idx; i < ENEMY_ZAKO_MAX; ++i) {
			enemy_zako_pool_[i].SetActive(false);
		}
		for (var i = dragon_head_idx; i < DRAGON_HEAD_MAX; ++i) {
			dragon_head_pool_[i].SetActive(false);
		}
		for (var i = dragon_body_idx; i < DRAGON_BODY_MAX; ++i) {
			dragon_body_pool_[i].SetActive(false);
		}
		for (var i = dragon_tail_idx; i < DRAGON_TAIL_MAX; ++i) {
			dragon_tail_pool_[i].SetActive(false);
		}

		// audio
		for (var i = 0; i < AUDIO_CHANNEL_MAX; ++i) {
			if (draw_buffer.se_[i] != DrawBuffer.SE.None) {
				switch (draw_buffer.se_[i]) {
					case DrawBuffer.SE.Bullet:
						audio_sources_bullet_[audio_source_bullet_index_].Play();
						++audio_source_bullet_index_;
						if (audio_source_bullet_index_ >= AUDIOSOURCE_BULLET_MAX) {
							audio_source_bullet_index_ = 0;
						}
						break;
					case DrawBuffer.SE.Explosion:
						audio_sources_explosion_[audio_source_explosion_index_].Play();
						++audio_source_explosion_index_;
						if (audio_source_explosion_index_ >= AUDIOSOURCE_EXPLOSION_MAX) {
							audio_source_explosion_index_ = 0;
						}
						break;
					case DrawBuffer.SE.Laser:
						audio_sources_laser_[audio_source_laser_index_].Play();
						++audio_source_laser_index_;
						if (audio_source_laser_index_ >= AUDIOSOURCE_LASER_MAX) {
							audio_source_laser_index_ = 0;
						}
						break;
					case DrawBuffer.SE.Shield:
						audio_sources_shield_[audio_source_shield_index_].Play();
						++audio_source_shield_index_;
						if (audio_source_shield_index_ >= AUDIOSOURCE_SHIELD_MAX) {
							audio_source_shield_index_ = 0;
						}
						break;
				}
				draw_buffer.se_[i] = DrawBuffer.SE.None;
			}
		}

		switch (draw_buffer.bgm_) {
			case DrawBuffer.BGM.Keep:
				break;
			case DrawBuffer.BGM.Stop:
				audio_sources_bgm_.Stop();
				is_bgm_playing_ = false;
				break;
			case DrawBuffer.BGM.Pause:
				if (is_bgm_playing_)
					audio_sources_bgm_.Pause();
				break;
			case DrawBuffer.BGM.Resume:
				if (is_bgm_playing_)
					audio_sources_bgm_.Play();
				break;
			case DrawBuffer.BGM.Battle:
				audio_sources_bgm_.Play();
				is_bgm_playing_ = true;
				break;
		}
		draw_buffer.bgm_ = DrawBuffer.BGM.Keep;
	}

	private void camera_update()
	{
	}

	private void unity_update()
	{
		camera_ = camera_holder_.GetComponent<Camera>();
		ProjectionMatrix = camera_.projectionMatrix;

#if !UNITY_EDITOR
		canvas_.SetActive(pause_);
#endif
		double render_time = update_time_ - dt_;
		render(ref draw_buffer_[rendering_front_]);
		Spark.Instance.render(rendering_front_, camera_, render_time);
		Beam.Instance.render(rendering_front_);
		Beam2.Instance.render(rendering_front_);
		WaterSurface.Instance.render(rendering_front_);
		WaterSurfaceRenderer.Instance.render(render_time);
		WaterSplash.Instance.render(rendering_front_, camera_, render_time);
		AuraEffect.Instance.render(render_time);
		Explosion.Instance.render(rendering_front_, camera_, render_time);
		Hahen.Instance.render(rendering_front_, render_time);
		Shield.Instance.render(rendering_front_, render_time);
		Debris.Instance.render(rendering_front_, camera_, render_time);
		Dust.Instance.render(rendering_front_, camera_, render_time);
		LightEnvironmentController.Instance.render(render_time);

		Sight.Instance.render(rendering_front_);
		MySprite.Instance.render(rendering_front_);
		MyFont.Instance.render(rendering_front_);
	}

	private void end_of_frame()
	{
		if (Time.deltaTime > 0) {
			++render_sync_frame_;
			if (!pause_) {
				++render_frame_;
			}
#if UTJ_MULTI_THREADED
			rendering_front_ = 1 - rendering_front_; // flip
			manual_reset_event_.Set();
#endif
			stopwatch_.Start();
		} else {
			stopwatch_.Stop();
		}
	}

	// The Update
	void Update()
	{
		PerformanceMeter.Instance.beginRender();
		if (!initialized_) {
			return;
		}
		PerformanceMeter.Instance.beginBehaviourUpdate();
		
		InputManager.Instance.update(rendering_front_);
#if !UTJ_MULTI_THREADED
		main_loop();
#endif
		unity_update();
		end_of_frame();
	}

	void LateUpdate()
	{
		if (!initialized_) {
			return;
		}
		camera_update();
		PerformanceMeter.Instance.endBehaviourUpdate();
	}

	public void OnPauseMenuAuto()
	{
		auto_ = true;
		spectator_mode_ = true;
		set_camera(spectator_mode_);
		Controller.Instance.set(auto_);
		pause_ = false;
	}
	
	public void OnPuaseMenuPlay()
	{
		auto_ = false;
		spectator_mode_ = false;
		set_camera(spectator_mode_);
		Controller.Instance.set(auto_);
		pause_ = false;
	}
}

} // namespace UTJ {

/*
 * End of SystemManager.cs
 */

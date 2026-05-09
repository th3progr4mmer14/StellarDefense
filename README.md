# StellarDefense 🚀

Juego 2D arcade estilo Space Invaders desarrollado en Unity 6 LTS con C#.

![Unity](https://img.shields.io/badge/Unity-6%20LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-purple?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Status](https://img.shields.io/badge/Status-En%20desarrollo-green)

---

## 🎮 Descripción

StellarDefense es un shooter arcade 2D donde el jugador debe defender la Tierra de oleadas de enemigos alienígenas. Inspirado en el clásico Space Invaders, el juego incorpora mecánicas modernas como sistema de combos, power-ups, efectos visuales y música dinámica.

---

## ✨ Características

### Gameplay
- 🚀 Nave del jugador con movimiento horizontal y disparo con cooldown
- 👾 3 tipos de enemigos (Basic, Fast, Tank) con stats configurables via ScriptableObjects
- 🎯 Formación de enemigos estilo Space Invaders clásico (movimiento lateral + bajada progresiva)
- 🌊 Sistema de waves escalable con dificultad progresiva (7 oleadas configuradas)
- ⚡ Power-ups: Escudo, Triple Disparo y Vida Extra
- 💥 Object Pooling para proyectiles (sin Instantiate/Destroy en runtime)

### Sistema de puntuación
- 🏆 Score en tiempo real con multiplicador de combo (hasta x10)
- 💾 High Score persistente entre sesiones (PlayerPrefs)
- ⏱️ Ventana de combo configurable

### Audio
- 🎵 Música dinámica contextual (MainMenu / Gameplay)
- 🔊 SFX para todas las acciones (disparo, hit, explosión, enemigos)
- 🎚️ AudioMixer con grupos Master/Music/SFX
- 🔧 Volúmenes persistentes entre sesiones

### UI Completa
- 📊 HUD con Score, High Score, Vidas (♥♥♥) y Wave actual
- 🏠 MainMenu con High Score y botones funcionales
- 💀 Game Over Screen con score final y detección de nuevo récord
- ⏸️ Pause Menu con Esc
- ⚙️ Settings Panel con sliders de volumen

### Polish Visual
- 🎨 Sprites pixel art del SpaceShooterAssetPack integrados en todas las entidades
- 🌌 Fondo estrellado tileado con efecto Tiled Draw Mode
- 💥 Partículas de explosión al matar enemigos
- 📳 Screen shake al recibir daño
- 🔢 Texto flotante con puntos y multiplicador de combo
- 🌟 Efecto de parpadeo (invulnerabilidad) al recibir daño
- 🔵 Efecto visual de escudo activo (parpadeo cian)
- 🎯 Rotación dinámica de proyectiles según dirección de vuelo

---

## 🏗️ Arquitectura

### Patrones de diseño utilizados
| Patrón | Aplicación |
|---|---|
| **Singleton** | GameManager, ScoreManager, AudioManager |
| **Object Pool** | ProjectilePool (proyectiles jugador y enemigos) |
| **Observer (Events)** | Enemy.OnAnyEnemyDestroyed, OnPlayerHit, OnLivesChanged |
| **State Machine** | GameManager (MainMenu, Playing, Paused, GameOver) |
| **ScriptableObjects** | GameSettings, EnemyData, WaveData |
| **Data-Driven Design** | Stats de enemigos y waves configurables sin recompilar |

### Estructura de carpetas
Assets/
├── Audio/
│   ├── Music/          (música de fondo)
│   └── SFX/            (efectos de sonido)
├── Input/              (PlayerControls.inputactions)
├── Prefabs/
│   ├── Effects/        (explosión, texto flotante)
│   ├── Enemies/        (BasicEnemy, FastEnemy, TankEnemy)
│   ├── Player/
│   ├── PowerUps/       (Shield, ExtraLife, TripleShot)
│   └── Projectiles/
├── Scenes/
│   ├── MainMenu.unity
│   └── Gameplay.unity
├── Scripts/
│   ├── Audio/          (AudioManager)
│   ├── Data/           (GameSettings ScriptableObject)
│   ├── Enemies/        (Enemy, EnemyFormation, WaveManager, EnemyData, WaveData)
│   ├── Managers/       (GameManager, ScoreManager, PolishManager)
│   ├── Player/         (PlayerController)
│   ├── PowerUps/       (PowerUp base, ShieldPowerUp, ExtraLifePowerUp, TripleShotPowerUp, PowerUpSpawner)
│   ├── Projectiles/    (Projectile, PlayerProjectile, EnemyProjectile, ProjectilePool)
│   ├── UI/             (HUDController, MainMenuController, GameOverController, PauseMenuController, SettingsController)
│   └── Utils/          (IDamageable, IPoolable, OffscreenCleaner, ScreenShake, ExplosionEffect, FloatingText)
├── Settings/
│   ├── Enemies/        (BasicEnemy.asset, FastEnemy.asset, TankEnemy.asset)
│   ├── GameSettings/   (DefaultGameSettings.asset)
│   └── Waves/          (Wave_01 a Wave_07.asset)
└── Sprites/            (SpaceShooterAssetPack: ships, enemies, projectiles, backgrounds, miscellaneous)

---

## 🛠️ Stack Técnico

| Tecnología | Versión | Uso |
|---|---|---|
| **Unity** | 6 LTS (6000.3.9f1) | Motor de juego |
| **C#** | 9.0 | Lenguaje de programación |
| **Input System** | 1.x | Sistema de input moderno |
| **TextMeshPro** | Incluido en Unity 6 | Textos de UI |
| **Universal RP** | 2D | Pipeline de renderizado |
| **AudioMixer** | Nativo | Sistema de audio |

---

## 🎮 Controles

| Acción | Tecla |
|---|---|
| Mover izquierda | `A` / `←` |
| Mover derecha | `D` / `→` |
| Disparar | `Espacio` |
| Pausar | `Esc` |

---

## 🚀 Cómo ejecutar

### Requisitos
- Unity 6 LTS (6000.3.9f1 o superior)
- Windows 10/11

### Pasos
1. Clona el repositorio:
```bash
git clone https://github.com/th3progr4mmer14/StellarDefense.git
```
2. Abre Unity Hub.
3. Click en **Add** → selecciona la carpeta del proyecto.
4. Abre el proyecto con Unity 6 LTS.
5. Abre la escena `Assets/Scenes/MainMenu.unity`.
6. Dale **Play**.

---

## 📋 Roadmap

- [x] Fase 1 — Configuración base (estructura, Input System, ScriptableObjects, Git)
- [x] Fase 2 — Gameplay Core (Player, Proyectiles, Enemigos, Formación, Waves)
- [x] Fase 3 — Managers globales (GameManager, ScoreManager, AudioManager)
- [x] Fase 4 — UI completa (HUD, MainMenu, GameOver, Pause, Settings)
- [x] Fase 5 — Audio completo (música contextual, SFX)
- [x] Fase 6 — Power-ups y Polish visual
- [x] Fase 7 — Integración de sprites y fondo (SpaceShooterAssetPack pixel art)
- [ ] Animaciones (wiggle de enemigos, propulsor del jugador, explosiones animadas)
- [ ] Más tipos de enemigos con comportamientos especiales (boss waves, kamikaze)
- [ ] Sistema de vidas con continues
- [ ] Build para WebGL

---

## 👨‍💻 Autor

**Eduardo Mollinedo**
- GitHub: [@th3progr4mmer14](https://github.com/th3progr4mmer14)

---

## 📄 Licencia

Este proyecto es de uso educativo y personal.
Assets gráficos: SpaceShooterAssetPack (uso libre para proyectos personales).

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

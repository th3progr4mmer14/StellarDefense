\# StellarDefense



Juego 2D arcade estilo Space Invaders desarrollado en Unity 6 LTS con C#.



\## Stack

\- Unity 6 LTS · URP 2D

\- C# (Mono / IL2CPP en build)

\- Input System (paquete nuevo)

\- TextMeshPro

\- uGUI (Canvas Scaler 1920×1080)



\## Estructura del proyecto



Assets/

├── Scripts/        Lógica organizada por dominio (Player, Enemies, ...)

├── Settings/       Instancias de ScriptableObject (datos)

├── Prefabs/        GameObjects reutilizables

├── Scenes/         MainMenu, Gameplay

├── Sprites/        Arte 2D

├── Audio/          Música y SFX

├── Materials/      Materiales 2D

└── Input/          PlayerControls.inputactions



\## Decisiones arquitectónicas (Fase 1)



1\. \*\*GameOver como overlay\*\*, no escena separada → conserva estado de la run y elimina una transición innecesaria.

2\. \*\*ScriptableObjects para todo el balance\*\*: GameSettings, EnemyData, WaveData. Permiten iterar valores sin recompilar.

3\. \*\*Namespaces por dominio\*\* (`StellarDefense.Player`, `.Enemies`, etc.) para evitar colisiones y dejar clara la propiedad del código.

4\. \*\*Layers separadas para proyectiles\*\* del jugador y enemigos: la matriz de colisiones hace todo el trabajo, el código no necesita comparar tags en hot paths.

5\. \*\*Input System nuevo\*\* desde el día 1: el legacy `Input` está en mantenimiento y no soporta bien rebinding ni multi-device.

6\. \*\*Comunicación por eventos C# (`Action`)\*\* entre managers y UI; ningún manager tiene referencia directa a otro salvo cuando es estrictamente necesario.



\## Cómo abrir

1\. Clonar el repositorio.

2\. Abrir con Unity Hub apuntando a Unity 6 LTS.

3\. Esperar a la compilación inicial (la carpeta `Library/` se regenera).

4\. Abrir `Assets/Scenes/MainMenu.unity` y darle a Play.



\## Roadmap

\- ✅ Fase 1 — Configuración base, escenas, ScriptableObjects, Input

\- ⬜ Fase 2 — Gameplay core (Player, proyectiles, enemigos, formación)

\- ⬜ Fase 3 — Managers globales (Game, Score, Audio, Save)

\- ⬜ Fase 4 — UI (menús, HUD, pausa, game over)

\- ⬜ Fase 5 — Audio (mixer, música, SFX)

\- ⬜ Fase 6 — Power-ups y polish


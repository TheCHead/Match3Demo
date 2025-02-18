#### Overview
This project is a Match-3 game prototype designed to showcase a scalable and extendable architecture. 

![match-3](https://github.com/user-attachments/assets/ad599d74-dfa6-4ace-a804-0ccae9d22c08)

#### Key Features
- Entity Component System (ECS): Uses Arch ECS to manage core gameplay loop.
- Game-changing passive items: some allow for new match patterns, some trigger unique effects.
- Modular UI System: Implements MVP architecture with a View and Presenter Factory, ensuring UI elements are loosely coupled and easily extendable.
- Dependency Injection for game services.
- Event-Driven State Management: A State Machine manages game flow, and an Event Bus facilitates communication between systems, preventing circular dependencies.

#### Technologies Used
- Unity (game engine)
- Arch ECS (core gameplay)
- Zenject (dependency injection)
- UniTask (Asynchronous programming)
- DOTween (transiitons and animations)

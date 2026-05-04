# Paint — Графічний редактор на базі WPF

Paint — це навчальний проєкт кросплатформового (архітектурно) графічного редактора, побудований на принципах чистого коду та чіткого розділення відповідальності між ядром програми та інтерфейсом.

## Запуск локально

1. Встановіть .NET 8 SDK або новіше.

2. Клонуйте репозиторій.

3. Відкрийте файл рішення .sln у Visual Studio 2022.

4. Натисніть F5 для запуску проєкту SWCPaint.Wpf.

## Функціонал програми
1. Інтерфейс користувача (UI)

    - Головне вікно:

        - Полотно для малювання.

        - Панель інструментів (олівець, пензель, гумка, геометричні фігури).

        - Панель керування шарами.

        - Вибір активних кольорів та товщини пензля.

    - Діалогове вікно створення проєкту:

        - Налаштування ширини та висоти полотна з валідацією (1 – 10 000 px).

        - Вибір фонового кольору.

    - Панель шарів:

        - Створення, видалення та зміна порядку шарів.

        - Керування видимістю окремих шарів.

2. Логіка редагування

    - Інструменти малювання:

        - Вільне малювання (Pencil, Brush).

        - Малювання примітивів (Rectangle, Circle, Line).

        - Інструмент гумка (Eraser) з підтримкою маскування шарів.

    - Система маскування: Реалізація "стирання" через PushMask на рівні рендерингу, що дозволяє неруйнівне редагування.

    - Історія дій: Повна підтримка операцій Undo/Redo для всіх маніпуляцій на полотні.

3. Збереження та експорт

    - Власний формат: Збереження структури проєкту (шари, фігури, налаштування) у JSON через систему серіалізації та завантаження експортованого проєкту у форматі JSON.

    - Експорт: Можливість збереження результату в растровому форматі PNG.

## Programming Principles

- SOLID:

    - Single Responsibility: Кожен клас виконує одну конкретну функцію. Наприклад, [`HistoryManager`](./SWCPaint.Core/Commands/HistoryManager.cs)) відповідає виключно за стек скасування/повторення дій, а [`JsonProjectSerializer`](./SWCPaint.Core/Services/Serialization/JsonProjectSerializer.cs)) відповідає лише за серіалізацію — тільки за збереження даних у форматі JSON. Використання MVVM у WPF-проєкті є прикладом SRP на рівні архітектури: View відповідає лише за відображення, ViewModel — за логіку взаємодії та стан UI, а Model — за чисту бізнес-логіку (геометрію та дані).

    - Open/Closed Principle (OCP): Клас Shape відкритий для розширення (створення нових фігур, як-от [`Polyline`](./SWCPaint.Core/Models/Shapes/Polyline.cs) або чи [`Ellipse`](./SWCPaint.Core/Models/Shapes/Ellipse.cs)), але закритий для модифікації — нам не потрібно змінювати код базового класу або логіку малювання, щоб додати новий тип об’єкта.

    - Liskov Substitution Principle (LSP): Будь-який нащадок [`LayerElement`](./SWCPaint.Core/Models/LayerElement.cs)) (фігура чи шлях ластика) може бути використаний замість базового класу без порушення роботи програми.

    - Interface Segregation Principle (ISP): Замість створення універсальних інтерфейсів використовуються вузькоспеціалізовані: [`IDrawingContext`](./SWCPaint.Core/Interfaces/IDrawingContext.cs), (тільки для малювання), [`IFileManager`](./SWCPaint.Core/Interfaces/Persistence/IFileManager.cs) (тільки для роботи з файлами), IUndoableCommand (тільки для команд з підтримкою історії).

    - Dependency Inversion Principle (DIP): Високорівнева логіка в Core не залежить від деталей реалізації у WPF. Наприклад, інструменти малюють через інтерфейс IDrawingContext, реалізація якого (WpfDrawingContext) знаходиться в шарі Infrastructure.

- YAGNI (You Ain't Gonna Need It): Реалізовано лише необхідний функціонал для роботи з шарами та фігурами без надмірної абстракції "на майбутнє". Використано єдиний [`ToolbarView`](./SWCPaint.Wpf/Views/ToolbarView.xaml) для всіх інструментів замість складної системи динамічних панелей для кожного окремого інструменту, оскільки поточні потреби проєкту цього не вимагають.

- DRY (Don't Repeat Yourself): Винесення спільної логіки команд у [`RelayCommand`](./SWCPaint.Wpf/Commands/RelayCommand.cs) та повторюваних властивостей (повідомлення про зміну стану) у [`BaseViewModel`](./SWCPaint.Wpf/ViewModels/BaseViewModel.cs) .Використання базових класів [`Shape`](./SWCPaint.Core/Models/Shapes/Shape.cs) та [`LayerElement`](./SWCPaint.Core/Models/LayerElement.cs) та [`BoxBoundedShape`](./SWCPaint.Core/Models/Shapes/BoxBoundedShape.cs) для уникнення дублювання коду малювання та розрахунку меж фігур.

- Encapsulation

    - Стан об'єктів (наприклад, [`Project`](./SWCPaint.Core/Models/Project.cs) або [`Shape`](./SWCPaint.Core/Models/Shapes/Shape.cs)) захищений від некоректних змін.

    - Використання властивостей з логікою валідації в сеттерах (наприклад, обмеження розміру полотна до 10 000 px або перевірка товщини лінії `Thickness` на від’ємні значення), що гарантує валідність стану об’єкта в будь-який момент.

    - Внутрішні структури даних (як-от списки елементів у Layer) приховані, а доступ до них надається через IReadOnlyList, що запобігає несанкціонованій модифікації ззовні.

- Separation of Concerns (SoC) Проєкт розділений на незалежні шари:

    - [Core](./SWCPaint.Core/): Чиста бізнес-логіка, моделі та інтерфейси.

    - [Infrastructure](./SWCPaint.Infrastructure/): Реалізація системних сервісів (WPF рендеринг, файлова система).

    - [WPF (Presentation)](./SWCPaint.Wpf/): Користувацький інтерфейс та логіка відображення.

## Design Patterns

- Strategy:

    Файли: [`ITool.cs`](./SWCPaint.Core/Interfaces/Tools/ITool.cs), [`PencilTool.cs`](./SWCPaint.Core/Tools/PencilTool.cs), [`ShapeTool.cs`](./SWCPaint.Core/Tools/ShapeTool.cs).

    Навіщо: Дозволяє змінювати алгоритм малювання (олівець, фігури, ластик) під час виконання програми. Головне вікно працює з абстракцією ITool, не знаючи деталей реалізації конкретного інструменту.

- Singleton 

    Файли:[`DrawingSettings.cs`](./SWCPaint.Core/Services/DrawingSettings.cs).

    Навіщо: Забезпечує єдину точку доступу до глобальних налаштувань малювання (колір, товщина пензля). Реалізовано через Lazy<T>, що гарантує потокобезпечність та відкладену ініціалізацію об'єкта. Події (SettingsChanged) дозволяють інструментам малювання миттєво реагувати на зміну кольору в UI.

- Factory

    Файли: [`ElementFactory.cs`](./SWCPaint.Core/Factories/ElementFactory.cs).

    Навіщо: Використовується під час десеріалізації проєкту. Фабрика аналізує тип елемента в JSON і створює відповідний об'єкт моделі (Rectangle, Ellipse, EraserPath), ізолюючи логіку створення від процесу завантаження файлу.

- Registry:

    Файли: [`ToolRegistry.cs`](./SWCPaint.Core/Tools/ToolRegistry.cs).

    Навіщо: Служить центральним сховищем доступних інструментів, забезпечуючи їх реєстрацію та доступ до них за ідентифікаторами.

- Visitor

    Файли: [`IElementVisitor.cs`](./SWCPaint.Core/Interfaces/IElementVisitor.cs), [`ProjectSerializationVisitor.cs`](./SWCPaint.Core/Services/Serialization/ProjectSerializationVisitor.cs).

    Навіщо: Дозволяє додавати нові операції над елементами (наприклад, експорт або збереження), не змінюючи класи самих елементів (Shape, LayerElement).

- Command:

    Файли: [`IUndoableCommand.cs`](./SWCPaint.Core/Interfaces/IUndoableCommand.cs), [`HistoryManager.cs`](./SWCPaint.Core/Commands/HistoryManager.cs), [`AddElementCommand.cs`](./SWCPaint.Core/Commands/AddElementCommand.cs), [`MoveLayerCommand.cs`](./SWCPaint.Core/Commands/MoveLayerCommand.cs), [`RelayCommand.cs`](./SWCPaint.Wpf/Commands/RelayCommand.cs).

    Навіщо: Використовується для реалізації системи Undo/Redo. Кожна дія (наприклад, переміщення шару чи малювання) інкапсулюється в об'єкт, який знає, як виконати операцію (Execute) та як її скасувати (Undo). Це дозволяє зберігати стек дій та легко відкочувати стан проєкту. Також у WPF ідокремлює дію (логіку) від ініціатора (кнопки в XAML), дозволяючи легко тестувати функціонал.

- Bridge:

    [`IDrawingContext.cs`](./SWCPaint.Core/Interfaces/IDrawingContext.cs), [`WpfDrawingContext.cs`](./SWCPaint.Infrastructure/Graphics/WpfDrawingContext.cs)
    Навіщо: Розділення абстракції малювання (Core) від конкретної реалізації рендерингу (WPF / DrawingContext).

## Refactoring Techniques

- Extract Method: Логіка ініціалізації у [`MainViewModel.cs`](./SWCPaint.Wpf/ViewModels/MainViewModel.cs) рознесена по методах `InitializeTools`, `InitializeCommands`, що покращує читабельність.

- Extract Superclass: Виділено абстрактний клас [`LayerElement.cs`](./SWCPaint.Core/Models/LayerElement.cs), що дозволило розв'язати конфлікт із гумкою [`EraserPath`](./SWCPaint.Core/Models/EraserPath.cs), яка має координати та межі, але не використовує Draw метод. Замість порушення принципу розділення інтерфейсу через пряме наслідування від Shape, спільну логіку (рух, межі, [`Visitor`](./SWCPaint.Core/Interfaces/IElementVisitor.cs)) винесено у базовий [`LayerElement.cs`](./SWCPaint.Core/Models/LayerElement.cs).

- Replace Magic Number with Symbolic Constant: Використання константи `MAX_DIMENSION` у [`Project.cs`](./SWCPaint.Core/Models/Project.cs) замість розкиданих по коду значень 10000.

- Introduce Parameter Object: Використання об'єктів на кшталт Point або Color замість передачі окремих координат x, y чи значень r, g, b у методи малювання.

- Simplify Conditional Expressions: Використання LINQ (наприклад, `.OfType<EraserPath>()`) у методах обробки шарів у [`Layer.cs`](./SWCPaint.Core/Models/Layer.cs) для фільтрації елементів шару замість ручного перебору в циклах з постійними перевірками типів

- Guard Clauses: Використовуються у сеттерах для миттєвого викидання виключень при порушенні меж полотна (1—10 000 px) чи товщини ліній в [`Shape.cs`](./SWCPaint.Core/Models/Shapes/Shape.cs), у маніпуляціях із шарами [`Project`](./SWCPaint.Core/Models/Project.cs) для ігнорування некоректних індексів або заборони видалення останнього шару, а також у рендерингу для швидкого пропуску прихованих об'єктів.

- Encapsulate Field: Всі внутрішні списки (наприклад, _layers) в [`Project`](./SWCPaint.Core/Models/Project.cs) приховані за IReadOnlyList, а зміна стану відбувається лише через публічні методи (`AddLayer`, `RemoveLayer`), що гарантує цілісність даних.

- Remove Dead Code: Видалено невикористані методи (наприклад, `IsHit` у базовому класі [`Shape`](./SWCPaint.Core/Models/Shapes/Shape.cs)), що дозволило спростити інтерфейс моделей та зменшити об'єм коду, який не несе функціонального навантаження.
# PizzaDeliveryApp

## Application Overview
* **Application Topic:** PizzaDeliveryApp
* **Title in WPF:** Pizza Delivery Service

---

## Technologies used
- .NET 10
- C#
- WPF (XAML)
- Class Library for Core (business logic)
- Resources.resx for localization
- LINQ for slices and simple queries
- Visual Studio 2022/2026 as recommended IDE

---
## How to run
Visual Studio (recommended):
1. Open PizzaDeliveryApp.slnx in Visual Studio 2022 or 2026.
2. In Solution Explorer set the WPF project as the Startup Project.
3. Build the solution (Build → Build Solution).
4. Run the application (F5 or Debug → Start Debugging).

## WPF and Usage (Interface, Dialogs, and Filtering)
Command line (dotnet CLI):
1. Open a terminal in the solution root folder.
2. Build: dotnet build PizzaDeliveryApp.slnx
3. Run the WPF project (replace the path with the actual WPF project file name):
   dotnet run --project ./WPF/YourWpfProject.csproj

---

## Core Requirements & Functionality

* Create a WPF application that manages records displayed in a DataGrid. DataGrid is a standard WPF component for tabular data display, supporting automatic column generation or manual configuration, as well as easy selection of a specific row for further editing or deletion.

### 1. Display
* The main window (`MainWindow.xaml`) contains a `<DataGrid>` component that is bound to data from the Core project. Upon program startup or after any modification in the table, it displays only current records (the first `count` elements of a fixed-size array).

### 2. Management (CRUD)
* **Creation (Create):** The "Add" button opens a dialog window to enter parameters for a new order.
* **Read (Read):** All current orders are clearly visible in the DataGrid table.
* **Delete (Delete):** The "Delete" button erases the selected record from the array followed by shifting elements.

### 3. Price and Time Calculation via Enum Combinations and a Switch Statement in Core
* Combinations are processed using a `switch` statement in the Core project. The `switch` statement contains at least six combinations of two enum values.

#### Two Enums (enum):
* `PizzaSize` (Pizza size: `Small`, `Medium`, `Large`).
* `DoughType` (Dough type: `Thin`, `Traditional`, `Fluffy`).

#### Logical Connection:
* Pizza cost and its preparation time are not set manually, but are determined programmatically based on the pair of characteristics ("size + dough") chosen by the client.

#### Implementation in Core:
In the public static class of the Core project, a special calculation method is created – `CalculatePriceAndPrepTime`:
* **Input Parameters:** Accepts `PizzaSize` (size) and `DoughType` (dough) enum values, as well as the number of sauces (`int sauceCount`).
* **Accounting for Additional Options (Business Logic):** Adds a fixed surcharge to the base cost for each selected sauce (for example, `sauceCount * 0.75m`).
* **Result Return:** The method returns a tuple consisting of two strongly typed values:
  * `TotalPrice` (decimal) — the final calculated pizza cost taking into account size, dough, and number of sauces.
  * `PrepMinutes` (int) — the exact preparation time in minutes determined by the parameter combination.

#### Switch Logic (Pattern Matching):
* **Pattern Matching:** The method takes a tuple of two arguments `(size, dough)` and checks their simultaneous match using modern `switch` syntax.
* **What is matched:** Each unique combination of pizza size (`PizzaSize`) and dough type (`DoughType`) is matched with its own pair of base values: `(basePrice, prepMinutes)`.
* **Default Value (_):** A default branch (fallback) is provided, which triggers if an unexpected combination is passed, protecting the application from crashing.

#### List of 6 Mandatory Combinations (and more):
* `(PizzaSize.Small, DoughType.Thin)` — Small pizza on thin crust (Base price: 7.00m, Time: 15 min).
* `(PizzaSize.Small, DoughType.Traditional)` — Small pizza on traditional crust (Price: 8.00m, Time: 18 min).
* `(PizzaSize.Medium, DoughType.Thin)` — Medium pizza on thin crust (Price: 10.00m, Time: 20 min).
* `(PizzaSize.Medium, DoughType.Traditional)` — Medium pizza on traditional crust (Price: 11.00m, Time: 22 min).
* `(PizzaSize.Large, DoughType.Traditional)` — Large pizza on traditional crust (Price: 14.00m, Time: 25 min).
* `(PizzaSize.Large, DoughType.Fluffy)` — Large pizza on fluffy crust (Price: 16.00m, Time: 30 min).
* *(Plus mandatory default branch `_ => (9.00m, 20)`)*.

### 4. Business Rule and DomainException
* A strict restriction on ingredients applies in the pizzeria: for a small pizza (`PizzaSize.Small`), the maximum allowable number of sauces (`sauceCount`) is exactly 3 pieces.

* **Step 1. Interface Protection:**
  * When the user selects the `PizzaSize.Small` size in the dialog box, the interface dynamically limits sauce selection: the maximum value of the counter/choice becomes equal to 3.
  * A hint from the resource file (`Resources.resx`) is immediately displayed nearby: *"Maximum 3 sauces available for a small pizza"*.

* **Step 2. Protection at the Core and Exception Level – Error Handler:**
  * A custom exception class `DomainException` is created in the Core project. Parameter validation is performed in the static order validation method before saving.
  * If this rule is violated, WPF catches `DomainException` in a `try-catch` block, accesses the `Resources.resx` file by the `Error_TooManySauces` key, and displays a warning pop-up window `MessageBox.Show(...)` with a warning icon.

---

## Data and Logic (Core and WPF Architecture)

### 1. Solution Structure and Project Connection
* **Solution** is divided into two independent projects:
  * **Core** — Class Library containing all business logic, structures, enums, data arrays, and validation rules.
  * **WPF** — Graphical application responsible for the user interface.
* **Connection:** The WPF project has a direct reference (`Project Reference`) to the Core project, which allows calling static methods and data structures.

### 2. Public Static Class of the Core Project – PizzaManager
Contains all static methods for managing the order array, price calculations (`CalculatePriceAndPrepTime`), and business rule validation:
* Fixed array `_orders` of 100 elements and `Count` counter.
* Addition methods (`AddOrder`) with shifting or limit checking.
* Deletion method (`DeleteOrder`) with a left-shifting element loop.
* Price and time calculation method `CalculatePriceAndPrepTime` using a switch expression (6+ combinations).

### 3. Used Types
* **Enums (enum):**
  * `PizzaSize` (`Small`, `Medium`, `Large`) — pizza size.
  * `DoughType` (`Thin`, `Traditional`, `Fluffy`) — dough type.
* **Structure (struct):**
  * `PizzaOrder` — structure for storing data about a single pizza order.
  * **Structure Fields:**
    * `int Id` (order number)
    * `string ClientName` (client name)
    * `PizzaSize Size` (size)
    * `DoughType Dough` (dough)
    * `int SauceCount` (number of sauces)
    * `decimal TotalPrice` (total price)
    * `int PrepMinutes` (preparation time)

### 4. Fixed Array and Counter (int count)
A fixed array and counter are declared in the Core static class:
```csharp
private static PizzaOrder[] _orders = new PizzaOrder[100];
public static int Count { get; private set; } = 0;
```

> [!NOTE]
> ### Array & Counter Details
> * How many real orders are currently recorded in the array.
> * Which exact free cell the next order should be placed into.

### Addition Logic with Size Check (`AddOrder`)
When the user clicks the "Save" button in the WPF dialog box, the addition method from the Core project is called.

* **Capacity check:** First, the method checks whether the array is completely full.
* **Writing to a cell:** If there is space, the new order is placed into the array strictly at the position of the current index `Count`.
* **Counter increment:** After successful writing, the counter increases by one, fixing that there are now more elements: `Count++`.

### Deletion Logic with Left Element Shift (`DeleteOrder`)
* **Index check:** The method ensures that the deleted index exists and is within the range from `0` to `Count - 1`.
* **Running a cyclical left shift:** The element located to the right of the deleted one is taken and dragged to the vacated space. And so on until the very end of the filled part of the array.
* **Clearing the tail and decreasing the counter:** After all elements have shifted to the left, the very last filled element (which is now duplicated) needs to be erased, and the counter decreased by 1.

---

## WPF and Usage (Interface, Dialogs, and Filtering)

* **Displaying Only the First Count Elements in DataGrid:**
  The main window (`MainWindow.xaml`) contains a `<DataGrid>` component whose data source (`ItemsSource`) is bound not to the entire 100-element array, but only to its active part. For this, a selection method from Core is used, which returns a slice of the array from index `0` to `Count - 1` (via the LINQ method `.Take(Count)`).
* **Adding and Editing via a Separate Dialog Window:**
  A separate dialog window (`OrderDialog.xaml`) is created for adding and modifying records. When clicking the "Add" button, the window opens empty (for entering a new order).
* **Deleting Records with Confirmation via MessageBox:**
  Before calling the deletion method from the Core project, the WPF application asks the user to confirm the action.
* **Search or Filtering Using a Loop (Array Traversal):**
  * An input field (search bar) has been added to the interface – by name only.
  * Loop traversal algorithm: no hidden databases are used for search — the search is performed by directly iterating over array elements in a `for` loop from `0` to `Count - 1` using `List<PizzaOrder>`.
* **Handling Specific Core Exceptions in WPF:**
  All calls to Core project methods (addition, modification) that can cause business errors are wrapped in `try-catch` block constructs.
* **Using the Resources.resx Resource File:**
  All interface text labels, success operation messages, deletion confirmation questions, and error keys are stored centrally in the `Resources.resx` file (with localization support). In WPF code, they are accessed via `Properties.Resources.Key`.
* **Updating DataGrid and Summary After Every Change:**
  Upon completion of any action (whether adding via dialog, editing, deleting, or searching), the interface update method (`RefreshDataGrid()`) is triggered, which re-fetches current `Count` data and recalculates summary indicators (for example, the total order amount).

---

## Project Composition

### Project 1: Core Class Library (Business Logic and Data)
*This project is responsible for data storage, structures, enums, and all calculation mathematics. It knows nothing about the WPF graphical interface.*

* **`PizzaSize.cs` (Enum):** Contains an enumeration of pizza size options (`Small`, `Medium`, `Large`). Provides strict typing for size selection.
* **`DoughType.cs` (Enum):** Contains an enumeration of dough types (`Thin`, `Traditional`, `Fluffy`). Used paired with size for price and time calculation.
* **`PizzaOrder.cs` (Structure / struct):** Describes the data model of a single pizza order. Fields: order ID, client name, size, dough, sauce count, total price, and preparation time.
* **`DomainException.cs` (Exception Class):** Custom exception for handling business rules (for example, sauce restriction for small pizza). Allows Core to "signal" WPF about logic rule violations.
* **`PizzaManager.cs` (Static Management Class):** Main business logic class:
  * Fixed array `_orders` of 100 elements and `Count` counter.
  * Addition methods (`AddOrder`) with shift or limit check.
  * Deletion method (`DeleteOrder`) with a left element shift loop.
  * Price and time calculation method `CalculatePriceAndPrepTime` using a switch expression (6+ combinations).

### Project 2: WPF Graphical Application (User Interface)
*This project references the Core project, and is responsible for rendering windows, user interaction, and exception handling.*

* **`Resources.resx` (Resource File):** Centralized storage of all application text strings (titles, deletion confirmation questions, error texts for `DomainException`). Eliminates text "hardcoding" in code and simplifies localization.
* **`MainWindow.xaml` (Main Window Layout):** Visual interface of the main screen. `<DataGrid>` component for displaying orders, control buttons ("Add", "Edit", "Delete"), search/filtering field, and summary statistics panel.
* **`MainWindow.xaml.cs` (Main Window Logic):** Code managing main window behavior. What it contains:
  * Table update method (`RefreshDataGrid`).
  * "Delete" button event handler with confirmation call via `MessageBox`.
  * Search/filtering logic using a loop over the Core array.
  * Opening dialog windows.
* **`OrderDialog.xaml` (Dialog Window Layout):** Window for entering and editing data of a specific order. Input fields for client name, drop-down lists (`ComboBox`) for size and dough, counter/field for sauce selection, and "Save" / "Cancel" buttons.
* **`OrderDialog.xaml.cs` (Dialog Window Logic):** Code managing the dialog window. What it contains:
  * Interface restriction (dynamic sauce restriction up to 3 for small pizza).
  * Error catching in a `try-catch` block when clicking "Save".
  * Displaying a `MessageBox` with text from `Resources.resx` upon catching `DomainException`.

---

## Application Color Palette
* **Primary Color (Primary / Accent):** Warm tomato red (`#E63946` or `#D90429`)
  * *Where it is used:* Action buttons ("Add Order"), table header, selected interface elements.
* **Secondary Color (Secondary / Backgrounds):** Creamy / Warm beige (`#F1FAEE` or `#FFF3B0`)
  * *Where it is used:* Window backgrounds, card backgrounds, or filtering panels.
* **Text and Border Color (Text / Neutral Dark):** Deep charcoal gray (`#1D3557` or `#2B2D42`)
  * *Where it is used:* Main text, headers, DataGrid element borders.
* **Success Operations / Statuses Color (Success):** Juicy grass green (`#2A9D8F`)
  * *Where it is used:* Order status indicators, success messages.

---
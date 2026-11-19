# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
# BUGS

| Rule violated | Where in the starter | How I found it | What I changed |
|---|---|---|---|
| R7 (touching intervals do not overlap) | `Services/BookingService.cs`, `Create`, the overlap check in the `foreach` . It used `<=` on both sides, so 10:00-12:00 and 12:00-14:00 were treated as a conflict. | Reading the code against R7, then testing two touching bookings in `roomhub.http`. | Overlap now lives in `DateRange.Overlaps` as `StartUtc < other.EndUtc && other.StartUtc < EndUtc` (strict comparison). |
| R7 / R8 (a cancelled booking frees its interval) | `Services/BookingService.cs`, same `foreach` . It used `ListForRoom`, which returns cancelled bookings too, so a cancelled booking still blocked the room. | Reading the code, then: create, cancel, create again in the same interval. | `IBookingRepository.GetActiveForRoomAsync` returns only `Active` bookings, and `Booking.Create` also checks `b.IsActive`. |
| R8 (cancelling twice gives 409) | `Services/BookingService.cs`, `Cancel`. It set `Status = Cancelled` without checking the current status, so a second cancel returned 204. | Reading the code, then calling cancel twice. | `Booking.Cancel(nowUtc)` returns `BookingErrors.AlreadyCancelled` (a Conflict, so 409) when the status is already `Cancelled`. |
| R5 (cannot start in the past) | Missing from `BookingService.Create`. | Checking every rule R1-R8 against the code one by one. | Added in `Booking.Create`. "Now" comes from `TimeProvider` in `CreateBookingHandler` and is passed in as `nowUtc`. |
| R6 (max duration 4 hours) | Missing from `BookingService.Create`. | Same checklist. | Added in `Booking.Create` through `Room.MaxBookingDuration` (4 hours, or 2 hours for rooms over 10 seats after R10). Exactly the limit is still allowed. |
| Persistence (data survives restart) | `Program.cs` registered in-memory repositories as singletons (`RoomRepository`, `BookingRepository`), so all data was lost on restart. | Reading `Program.cs`, and the comment "in-memory data" in the repositories. | EF Core + SQLite: `RoomHubDbContext`, configurations, repositories, `DatabaseInitializer`. |


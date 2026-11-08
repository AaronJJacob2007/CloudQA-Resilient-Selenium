# Approach and Limitations

## Problem

Traditional Selenium tests often use fixed element IDs, names, CSS classes or positional XPath selectors. Such locators can break when page markup changes.

The objective of this project is to identify the intended form controls through their meaning and relationships rather than their original identifiers.

## Locator Strategy

**First Name**

The application finds the visible First Name label and prioritizes its current `for` association with an input.

When no explicit association is available, it searches nearby containers. If several plausible controls exist, it reports ambiguity rather than guessing.

**Gender**

The application finds the Gender label, searches the associated nearby container for radio inputs, and identifies the requested option using its value or adjacent visible text.

It rejects duplicate matching options.

**State**

The application finds the State label and identifies an associated native HTML dropdown. It selects India and verifies the selected option.

Ambiguous dropdown matches cause a controlled failure.

## Verification

The automation does not consider an interaction successful merely because no exception occurred.

It checks:

- The text field's actual value
- The radio button's selected state
- The dropdown's selected option

The application returns a nonzero exit code on failure.

## Resilience Testing

The project includes local HTML fixtures covering:

- Changed IDs, names and classes
- Reordered form fields
- Minor wrapper structure changes
- Similar-looking decoy fields
- Duplicate labels and matching controls

These fixtures are executed through Selenium using xUnit.

## Known Limitations

1. Visible labels must remain recognizable.
2. Unsupported or significantly different HTML structures may require additional locator strategies.
3. The implementation expects native radio inputs and HTML select dropdowns; custom JavaScript widgets are not supported.
4. If a label has no explicit association and several nearby controls are plausible, the automation intentionally fails.
5. Browser availability, page loading and JavaScript changes can affect execution.
6. Passing the provided fixtures does not guarantee compatibility with every possible unseen DOM modification.

## Design Decision

The implementation prefers a clear, diagnosable failure over interacting with the wrong field.

This is a deliberate trade-off between resilience, correctness and maintainability.
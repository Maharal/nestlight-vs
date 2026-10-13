using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>The description of one word (<c>select</c>, <c>div</c>, <c>href</c>, <c>display</c>...), as opposed to the one of its kind of token.</summary>
    internal static class TokenDescriptions
    {
        /// <summary>The description of the word for the kind of token, or null when there is none.</summary>
        public static string Of(string classificationType, string word)
        {
            if (classificationType == null || word == null) return null;
            word = word.Trim();
            if (word.Length == 0) return null;

            string found;
            if (classificationType == ClassificationNames.SqlKeyword)
                return Sql.TryGetValue(word, out found) ? found : null;
            if (classificationType == ClassificationNames.Tag)
                return Tags.TryGetValue(word, out found) ? found : null;
            if (classificationType == ClassificationNames.Attribute || classificationType == ClassificationNames.AttributeBoolean)
                return Attributes.TryGetValue(word, out found) ? found : null;
            if (classificationType == ClassificationNames.AttributeEvent)
                return EventHandler(word);
            if (classificationType == ClassificationNames.CssProperty)
                return CssProperties.TryGetValue(word, out found) ? found : CssLogical(word);
            if (classificationType == ClassificationNames.CssAtRule)
                return AtRules.TryGetValue(word.TrimStart('@'), out found) ? found : null;
            if (classificationType == ClassificationNames.CssPseudo)
                return Pseudos.TryGetValue(word.TrimStart(':'), out found) ? found : null;
            if (classificationType == ClassificationNames.CssFunction)
                return CssFunctions.TryGetValue(word.TrimEnd('('), out found) ? found : null;
            if (classificationType == ClassificationNames.GqlOperation || classificationType == ClassificationNames.GqlKeyword)
                return Graphql.TryGetValue(word, out found) ? found : null;
            return null;
        }

        private static string EventHandler(string word)
        {
            string name = word.TrimStart('@', '(').TrimEnd(')');
            if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase) && name.Length > 2) name = name.Substring(2);
            if (name.Length == 0) return null;
            return "Runs the handler when the \"" + name.ToLowerInvariant() + "\" event fires on the element.";
        }

        private static string CssLogical(string word)
        {
            string lower = word.ToLowerInvariant();
            if (lower.StartsWith("--")) return null;
            string[] sides = { "top", "right", "bottom", "left" };
            foreach (string family in new[] { "margin", "padding" })
                foreach (string side in sides)
                    if (lower == family + "-" + side)
                        return "Sets the " + family + " on the " + side + " side of the element.";
            if (lower.StartsWith("border-") && lower.EndsWith("-color")) return "Sets the color of one border of the element.";
            if (lower.StartsWith("border-") && lower.EndsWith("-width")) return "Sets the thickness of one border of the element.";
            if (lower.StartsWith("border-") && lower.EndsWith("-style")) return "Sets the line style of one border of the element.";
            if (lower.StartsWith("border-") && lower.EndsWith("-radius")) return "Rounds one corner of the element's border.";
            if (lower.StartsWith("animation-")) return "Controls one aspect of a CSS animation.";
            if (lower.StartsWith("transition-")) return "Controls one aspect of a CSS transition.";
            if (lower.StartsWith("font-")) return "Controls one aspect of the font used to render text.";
            if (lower.StartsWith("text-")) return "Controls one aspect of how text is rendered.";
            if (lower.StartsWith("grid-")) return "Controls one aspect of a grid layout.";
            if (lower.StartsWith("scroll-")) return "Controls one aspect of scrolling behavior.";
            if (lower.StartsWith("mask-")) return "Controls one aspect of the mask applied to the element.";
            if (lower.StartsWith("background-")) return "Controls one aspect of the element's background.";
            if (lower.StartsWith("stroke-")) return "Controls one aspect of how an SVG shape is outlined.";
            return null;
        }

        private static Dictionary<string, string> Parse(string table)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string raw in table.Split('\n'))
            {
                string line = raw.Trim();
                int bar = line.IndexOf('|');
                if (bar <= 0) continue;
                string description = line.Substring(bar + 1).Trim();
                foreach (string key in line.Substring(0, bar).Split(' '))
                    if (key.Length > 0) map[key] = description;
            }
            return map;
        }

        private static readonly Dictionary<string, string> Sql = Parse(@"
select|Retrieves rows from one or more tables. Lists the columns or expressions to return.
from|Names the table, view or subquery the rows are read from.
where|Filters the rows, keeping only those for which the condition is true.
insert|Adds new rows to a table.
into|Names the table (and optionally the columns) that receives inserted rows.
values|Supplies the literal rows to insert, or the rows of a values list.
update|Changes the values of existing rows in a table.
set|Assigns new values to columns in an UPDATE, or changes a session setting.
delete|Removes rows from a table.
create|Creates a new database object, such as a table, view or index.
alter|Changes the definition of an existing database object.
drop|Permanently removes a database object, such as a table or index.
table|Refers to a table, the basic structure that stores rows of data.
view|Refers to a stored query that behaves like a virtual table.
index|Refers to a structure that speeds up lookups on one or more columns.
database|Refers to a database, a named collection of schemas and tables.
schema|Refers to a namespace that groups tables and other objects.
join|Combines rows from two tables based on a related condition.
inner|Used with JOIN to keep only the rows that match on both sides.
left|Used with JOIN to keep every row of the left table, matched or not.
right|Used with JOIN to keep every row of the right table, matched or not.
full|Used with JOIN to keep every row of both tables, matched or not.
outer|Optional word in LEFT, RIGHT and FULL joins that keeps the unmatched rows.
cross|Used with JOIN to produce every combination of rows from both tables.
natural|Used with JOIN to join on all columns that have the same name in both tables.
on|Gives the condition on which a JOIN matches rows (or the target of CREATE INDEX).
using|Joins on the named columns that exist in both tables.
group|Used in GROUP BY to collapse rows that share values into one row per group.
by|Follows GROUP or ORDER to name the columns used for grouping or sorting.
order|Used in ORDER BY to sort the result.
having|Filters groups after GROUP BY, usually on an aggregate such as COUNT.
limit|Restricts the number of rows returned.
offset|Skips a number of rows before returning the result.
fetch|Retrieves a number of rows from the result (FETCH FIRST n ROWS) or from a cursor.
first|Used with FETCH, or NULLS FIRST, to take or place the first rows.
next|Used with FETCH to take the next rows of the result.
rows row|Refers to rows of the result, as in FETCH FIRST 10 ROWS ONLY.
only|Ends FETCH FIRST n ROWS ONLY, or restricts an operation to the named table alone.
union|Combines the results of two queries, removing duplicate rows.
all|Keeps duplicates (UNION ALL) or compares against every value of a subquery.
distinct|Removes duplicate rows from the result.
intersect|Returns only the rows present in both query results.
except|Returns the rows of the first query that are absent from the second.
as|Gives an alias to a column or table, or names the type in a CAST.
and|Logical operator that is true only when both conditions are true.
or|Logical operator that is true when at least one condition is true.
not|Negates a condition.
null|The absence of a value. Compare it with IS NULL, never with =.
is|Compares against NULL, TRUE or FALSE, as in IS NULL.
in|Tests whether a value matches any value in a list or subquery.
exists|Tests whether a subquery returns at least one row.
between|Tests whether a value lies inside a range, both ends included.
like|Matches text against a pattern, where % is any sequence and _ any one character.
ilike|Case-insensitive version of LIKE (PostgreSQL).
similar|Used in SIMILAR TO, a pattern match that mixes LIKE and regular expressions.
case|Starts a conditional expression that returns the value of the first matching WHEN.
when|Gives a condition inside CASE, or the condition of a MERGE branch.
then|Gives the value returned when the WHEN condition is true.
else|Gives the value returned when no WHEN condition is true.
end|Closes a CASE expression or a block.
asc|Sorts in ascending order (the default).
desc|Sorts in descending order.
primary|Used in PRIMARY KEY, the column that uniquely identifies each row.
key|Used in PRIMARY KEY and FOREIGN KEY to name the identifying columns.
foreign|Used in FOREIGN KEY, a column that points to a row of another table.
references|Names the table and column that a foreign key points to.
unique|Constraint that forbids duplicate values in the column.
check|Constraint that requires every row to satisfy a condition.
default|The value used when none is given for a column.
constraint|Gives a name to a rule that the table data must follow.
cascade|Propagates a delete, update or drop to the dependent objects.
truncate|Quickly removes all rows from a table.
begin|Starts a transaction or a block of statements.
commit|Makes the changes of the current transaction permanent.
rollback|Undoes the changes of the current transaction.
transaction|Refers to a group of statements that succeed or fail together.
savepoint|Marks a point in a transaction that can be rolled back to.
with|Defines a named subquery (common table expression) used by the query that follows.
recursive|Lets a WITH query refer to itself, to walk hierarchies and graphs.
over|Defines the window of rows a window function is computed across.
partition|Splits the rows into groups inside a window, as in PARTITION BY.
window|Defines a named window specification for window functions.
returning|Returns the affected rows of an INSERT, UPDATE or DELETE (PostgreSQL).
true false|Boolean literal.
add|Adds a column or constraint in ALTER TABLE.
column|Refers to a column of a table.
rename|Changes the name of a table, column or other object.
to|Gives the new name in RENAME, or the grantee in GRANT.
grant|Gives privileges on an object to a user or role.
revoke|Takes privileges on an object back from a user or role.
if|Makes a statement conditional, as in IF EXISTS or IF NOT EXISTS.
replace|Replaces an existing object (CREATE OR REPLACE), or the REPLACE function.
temp temporary|Creates an object that lasts only for the session.
explain|Shows the execution plan the database would use for a query.
analyze|Collects statistics, or runs the query and reports actual execution times.
vacuum|Reclaims the storage of dead rows (PostgreSQL).
declare|Declares a variable or a cursor.
cast|Converts a value to another data type.
any some|Compares a value with at least one value of a subquery.
escape|Defines the escape character of a LIKE pattern.
lateral|Lets a subquery in FROM refer to the columns of the tables that come before it.
merge|Inserts, updates or deletes rows in one statement depending on a match with a source.
matched|Used in MERGE to choose the action for matching or non-matching rows.
do|Gives the action to run, as in ON CONFLICT DO NOTHING / DO UPDATE.
nothing|Used in ON CONFLICT DO NOTHING to skip the conflicting row.
conflict|Used in ON CONFLICT to handle a violation of a unique constraint during INSERT.
for|Used in FOR UPDATE and FOR SHARE to lock the selected rows, or in loops.
share|Used in FOR SHARE to lock rows for reading while blocking changes.
lock|Locks a table or rows against concurrent changes.
unlock|Releases a lock.
procedure|Refers to a stored routine that is run with CALL.
function|Refers to a stored routine that returns a value.
trigger|Refers to code that runs automatically when a table changes.
returns|Declares the data type a function returns.
return|Returns a value from a function.
language|Names the language a routine is written in.
execute|Runs a prepared statement or a routine.
call|Runs a stored procedure.
use|Selects the database that later statements apply to.
top|Restricts the number of rows returned (SQL Server).
int integer|32-bit whole number.
bigint|64-bit whole number.
smallint|16-bit whole number.
tinyint|Very small whole number (8-bit).
varchar nvarchar|Variable-length text with a maximum length.
char|Fixed-length text.
text|Text of unlimited (or very large) length.
boolean bool|True or false value.
date|Calendar date without time.
time|Time of day without date.
timestamp timestamptz|Date and time (timestamptz includes the time zone).
interval|A span of time, such as 3 days.
decimal numeric|Exact number with fixed precision, suited to money.
float real double|Approximate floating-point number.
precision|Part of DOUBLE PRECISION, the 64-bit floating-point type.
serial bigserial|Auto-incrementing integer column.
uuid|Universally unique identifier.
json jsonb|JSON document column (jsonb is stored in a binary, indexable form).
bytea blob|Raw binary data.
money|Currency amount.
");

        private static readonly Dictionary<string, string> Tags = Parse(@"
a|Anchor: a hyperlink to another page, a file or a place in the page.
abbr|An abbreviation or acronym; the title attribute can give its expansion.
address|Contact information for the author or owner of the nearest article or page.
area|A clickable region inside an image map.
article|A self-contained piece of content, such as a post or a card.
aside|Content related to, but separate from, the main content, such as a sidebar.
audio|Embeds sound content with playback controls.
b|Text drawn to attention without extra importance, usually shown in bold.
base|Sets the base URL that relative links in the document are resolved against.
blockquote|A block of text quoted from another source.
body|The visible content of the document.
br|A line break inside text.
button|A clickable button that can submit a form or run an action.
canvas|A bitmap area drawn with scripts, for graphics and games.
caption|The title of a table.
cite|The title of a creative work being cited.
code|A fragment of computer code, shown in a monospaced font.
col|Describes the properties of one column in a table column group.
colgroup|Groups table columns so they can be styled together.
data|Links content to a machine-readable value.
datalist|A list of suggested values for an input.
dd|The description (value) of a term in a description list.
del|Text that has been deleted from the document.
details|A disclosure widget that shows or hides extra content.
dfn|The term being defined.
dialog|A dialog box or modal window.
div|A generic block container with no meaning of its own, used for grouping and layout.
dl|A description list of terms and descriptions.
dt|A term in a description list.
em|Emphasized text, usually shown in italics.
embed|Embeds external content, such as a plugin.
fieldset|Groups related form controls and labels.
figcaption|The caption of a figure.
figure|Self-contained content, such as an illustration, with an optional caption.
footer|The footer of the page or of a section.
form|A form that collects user input and submits it.
h1 h2 h3 h4 h5 h6|A section heading; h1 is the most important and h6 the least.
head|Metadata of the document, such as the title, links and scripts.
header|Introductory content of the page or of a section.
hgroup|Groups a heading with its subtitle.
hr|A thematic break between paragraphs, drawn as a horizontal rule.
html|The root element of an HTML document.
i|Text in an alternate voice or mood, usually shown in italics.
iframe|Embeds another web page inside the current one.
img|Embeds an image.
input|An interactive form control; its type attribute decides what it looks like and does.
ins|Text that has been inserted into the document.
kbd|Text the user should type on the keyboard.
label|A caption for a form control; clicking it focuses the control.
legend|The caption of a fieldset.
li|A list item of an ol, ul or menu.
link|Links the document to an external resource, most often a stylesheet.
main|The main content of the page, unique to that page.
map|Defines an image map with clickable areas.
mark|Highlights text for reference.
menu|A list of commands, an alternative to ul for toolbars.
meta|Metadata that other elements cannot express, such as charset or viewport.
meter|A scalar measurement inside a known range, such as disk usage.
nav|A section with navigation links.
noscript|Content shown when scripting is disabled.
object|Embeds an external resource, such as a PDF or a plugin.
ol|An ordered (numbered) list.
optgroup|Groups options inside a select.
option|An option inside a select, datalist or optgroup.
output|The result of a calculation or user action.
p|A paragraph of text.
picture|Provides alternative image sources for different screens.
pre|Preformatted text, with whitespace and line breaks kept.
progress|Shows the completion progress of a task.
q|A short inline quotation.
rp|Fallback parentheses for browsers that do not support ruby text.
rt|The pronunciation or annotation of ruby text.
ruby|Annotations for East Asian typography.
s|Text that is no longer accurate or relevant, shown struck through.
samp|Sample output from a computer program.
script|Embeds or references executable code, usually JavaScript.
search|A section that contains the controls of a search or filter.
section|A thematic grouping of content, usually with a heading.
select|A drop-down list of options.
slot|A placeholder inside a web component that you fill with your own markup.
small|Side comments and fine print.
source|A media or image source for audio, video or picture.
span|A generic inline container with no meaning of its own, used for styling.
strong|Text of strong importance, usually shown in bold.
style|Embeds CSS rules in the document.
sub|Subscript text.
summary|The visible heading of a details element.
sup|Superscript text.
table|Tabular data in rows and columns.
tbody|The body rows of a table.
td|A data cell of a table row.
template|Markup that is not rendered until it is cloned by a script.
textarea|A multi-line text input.
tfoot|The footer rows of a table.
th|A header cell of a table.
thead|The header rows of a table.
time|A date or time, optionally in machine-readable form.
title|The title of the document, shown in the browser tab.
tr|A row of a table.
track|A text track, such as subtitles, for audio and video.
u|Text with an unarticulated annotation, usually underlined.
ul|An unordered (bulleted) list.
var|A variable in a mathematical or programming expression.
video|Embeds a video with playback controls.
wbr|A position where the browser may break a long word.
svg|The root of an SVG graphic, or a container for one.
path|An SVG shape described by a series of drawing commands.
circle|An SVG circle.
rect|An SVG rectangle.
line|An SVG straight line.
polyline|An SVG shape made of connected straight lines, left open.
polygon|An SVG closed shape made of straight lines.
ellipse|An SVG ellipse.
g|An SVG group; attributes set on it apply to all its children.
defs|Holds SVG definitions that are reused later, such as gradients.
use|Reuses an SVG element defined elsewhere.
symbol|A reusable SVG template drawn through use.
tspan|A span of text inside an SVG text element.
text|An SVG text element.
lineargradient|An SVG gradient that blends colors along a line.
radialgradient|An SVG gradient that blends colors outward from a point.
stop|A color stop inside an SVG gradient.
clippath|An SVG shape that hides whatever lies outside it.
mask|An SVG mask that controls the transparency of the element it is applied to.
filter|An SVG filter effect applied to graphics.
");

        private static readonly Dictionary<string, string> Attributes = Parse(@"
class|One or more CSS class names of the element, separated by spaces.
id|A unique identifier for the element in the document.
style|Inline CSS declarations applied to the element.
title|Extra information shown as a tooltip when the pointer rests on the element.
lang|The language of the element's content, such as en or pt-BR.
dir|The text direction: ltr, rtl or auto.
hidden|Hides the element from the page.
tabindex|Sets whether and in what order the element receives keyboard focus.
role|The ARIA role that tells assistive technology what the element is.
draggable|Whether the element can be dragged.
contenteditable|Makes the content of the element editable by the user.
accesskey|A keyboard shortcut to activate or focus the element.
slot|The name of the slot of a web component this element is placed in.
spellcheck|Whether the browser may check the spelling of the content.
translate|Whether the content should be translated.
aria-label|Gives the element an accessible name when it has no visible text.
aria-hidden|Hides the element from assistive technology.
aria-expanded|Whether the element it controls is expanded or collapsed.
aria-labelledby|The id of the element that labels this one.
aria-describedby|The id of the element that describes this one.
aria-live|Tells assistive technology how to announce changes to this region.
href|The URL the link or resource points to.
target|Where to open the link or show the response, such as _blank for a new tab.
rel|The relationship between the current document and the linked resource.
download|Makes the link download the file instead of navigating to it.
hreflang|The language of the linked resource.
type|The type of the element: input kind, button kind, or MIME type of a resource.
src|The URL of the resource to embed, such as an image, script or video.
alt|Alternative text shown or read out when an image cannot be displayed.
width|The width of the element, in pixels for most elements.
height|The height of the element, in pixels for most elements.
loading|Whether to load the resource now (eager) or when it nears the viewport (lazy).
srcset|A set of image sources for different screen sizes and densities.
sizes|The display size of the image for each media condition, used with srcset.
name|The name of the control, sent with its value when a form is submitted.
value|The current or initial value of the control.
placeholder|A hint shown in the control while it is empty.
required|The control must be filled before the form can be submitted.
disabled|Disables the control: it cannot be used or focused and is not submitted.
checked|Whether a checkbox or radio button starts selected.
readonly|The value can be seen and submitted but not edited.
min|The minimum allowed value.
max|The maximum allowed value.
step|The granularity of allowed numeric values.
pattern|A regular expression the value must match.
maxlength|The maximum number of characters allowed.
minlength|The minimum number of characters required.
autocomplete|Whether and how the browser may fill in the control automatically.
autofocus|Focuses the control as soon as the page loads.
form|The id of the form the control belongs to, when it is outside it.
action|The URL that processes the form submission.
method|The HTTP method used to submit the form, get or post.
enctype|How the form data is encoded when submitted.
novalidate|Turns off the browser's validation when the form is submitted.
for|The id of the control that the label belongs to, or that the output reports on.
multiple|Allows selecting or entering more than one value.
selected|Whether the option starts selected.
label|A text label for the option, track or group.
rows|The number of visible text lines of the textarea.
cols|The visible width of the textarea in characters.
colspan|The number of columns the cell spans.
rowspan|The number of rows the cell spans.
headers|The ids of the header cells that apply to this cell.
scope|Which cells a header cell applies to: row, col, rowgroup or colgroup.
align|The horizontal alignment of the content (obsolete; use CSS).
media|The media condition under which the resource applies.
as|The kind of content a preloaded resource is, such as script or style.
crossorigin|Whether to send credentials and how to handle CORS for the request.
integrity|A hash that the fetched resource must match.
content|The value of the metadata named by name or http-equiv.
charset|The character encoding of the document or script.
http-equiv|The HTTP header the content attribute stands in for.
async|Loads the script in parallel and runs it as soon as it is ready.
defer|Loads the script in parallel and runs it after the document is parsed.
allow|The features, such as camera or fullscreen, the embedded frame may use.
sandbox|Restricts what the embedded content may do.
controls|Shows the browser's playback controls.
autoplay|Starts playback automatically.
loop|Restarts playback when it ends.
muted|Starts with the sound off.
poster|An image shown until the video starts playing.
preload|How much of the media to load in advance.
start|The number the ordered list starts counting from.
reversed|Numbers the list in descending order.
viewbox|The position and size of the SVG's coordinate system: min-x min-y width height.
xmlns|The XML namespace of the document.
fill|The color that fills an SVG shape.
stroke|The color of the outline of an SVG shape.
stroke-width|The thickness of the outline of an SVG shape.
preserveaspectratio|How an SVG is scaled to fit its viewport.
d|The path data: the drawing commands of an SVG path.
cx cy|The center coordinate of an SVG circle or ellipse.
r|The radius of an SVG circle.
rx ry|The corner radius, or ellipse radius, of an SVG shape.
x y|The position of an SVG element.
x1 y1 x2 y2|The start (1) and end (2) coordinates of an SVG line.
transform|A list of transformations applied to an SVG element.
font-size|The size of the font used for SVG text.
open|Whether the details or dialog is open.
data-id|A custom data attribute, readable in JavaScript through dataset.
");

        private static readonly Dictionary<string, string> CssProperties = Parse(@"
align-content|Aligns the lines of a flex or grid container along the cross axis when there is extra space.
align-items|Aligns the items of a flex or grid container along the cross axis.
align-self|Overrides align-items for a single flex or grid item.
all|Resets all properties of the element to their initial or inherited values.
animation|Shorthand that applies a keyframe animation: name, duration, timing function, delay, iterations and more.
animation-delay|How long to wait before the animation starts.
animation-direction|Whether the animation plays forward, backward or alternates.
animation-duration|How long one cycle of the animation takes.
animation-fill-mode|Which styles apply before the animation starts and after it ends.
animation-iteration-count|How many times the animation repeats.
animation-name|The name of the @keyframes rule to use.
animation-play-state|Pauses or runs the animation.
animation-timing-function|The easing curve of the animation.
aspect-ratio|The preferred width to height ratio of the box, such as 16 / 9.
backdrop-filter|Applies graphical effects, such as blur, to the area behind the element.
backface-visibility|Whether the back face of a rotated element is visible.
background|Shorthand for the background: color, image, position, size, repeat and more.
background-attachment|Whether the background scrolls with the page or stays fixed.
background-blend-mode|How the background layers blend with each other.
background-clip|How far the background extends within the element.
background-color|The background color of the element.
background-image|One or more background images or gradients.
background-origin|The area the background position is relative to.
background-position|The starting position of the background image.
background-repeat|Whether and how the background image repeats.
background-size|The size of the background image.
border|Shorthand for the border: width, style and color.
border-bottom border-left border-right border-top|Shorthand for the width, style and color of one side of the border.
border-collapse|Whether table borders are merged or kept separate.
border-color|The color of the border.
border-image|Uses an image as the border.
border-radius|Rounds the corners of the element.
border-spacing|The distance between the borders of adjacent table cells.
border-style|The line style of the border, such as solid or dashed.
border-width|The thickness of the border.
bottom|The offset of a positioned element from the bottom edge of its container.
box-shadow|Adds one or more shadows around the element's box.
box-sizing|Whether width and height include the padding and border (border-box) or not (content-box).
caption-side|Which side of the table the caption goes on.
caret-color|The color of the text insertion cursor.
clear|Moves the element below floated elements on the given side.
clip-path|Shows only the part of the element inside a shape.
color|The color of the text (and of currentColor).
column-count|The number of columns the content is divided into.
column-gap|The gap between columns.
columns|Shorthand for column-width and column-count.
content|The generated content of ::before and ::after.
counter-increment|Increases a CSS counter.
counter-reset|Creates or resets a CSS counter.
cursor|The shape of the mouse pointer over the element.
direction|The text direction of the element.
display|The kind of box the element generates and how its children are laid out, such as block, inline, flex or grid.
empty-cells|Whether borders and background of empty table cells are drawn.
fill|The color that fills an SVG shape.
filter|Applies graphical effects, such as blur or grayscale, to the element.
flex|Shorthand for flex-grow, flex-shrink and flex-basis.
flex-basis|The initial size of a flex item before free space is distributed.
flex-direction|The main axis of a flex container: row or column.
flex-flow|Shorthand for flex-direction and flex-wrap.
flex-grow|How much of the free space the flex item takes.
flex-shrink|How much the flex item shrinks when there is not enough space.
flex-wrap|Whether flex items stay on one line or wrap onto several.
float|Takes the element out of the flow and places it to the left or right so text wraps around it.
font|Shorthand for font style, weight, size, line height and family.
font-family|The list of fonts to use, in order of preference.
font-size|The size of the text.
font-style|Normal, italic or oblique text.
font-variant|Alternate glyph forms, such as small caps.
font-weight|The boldness of the text.
gap|The space between rows and columns in flex and grid layouts.
grid|Shorthand for the explicit and implicit grid properties.
grid-area|Places a grid item in a named area or between grid lines.
grid-auto-columns|The size of columns created implicitly.
grid-auto-flow|How auto-placed items fill the grid.
grid-auto-rows|The size of rows created implicitly.
grid-column|Shorthand for the start and end column lines of a grid item.
grid-row|Shorthand for the start and end row lines of a grid item.
grid-template|Shorthand for rows, columns and areas of a grid.
grid-template-areas|Names the areas of a grid with a text layout.
grid-template-columns|The size of each column of the grid.
grid-template-rows|The size of each row of the grid.
height|The height of the element.
hyphens|Whether words may be split at line ends with hyphens.
inset|Shorthand for top, right, bottom and left.
isolation|Whether the element creates its own stacking context.
justify-content|Aligns the items of a flex or grid container along the main axis.
justify-items|Aligns grid items along the inline axis.
justify-self|Aligns a single grid item along the inline axis.
left|The offset of a positioned element from the left edge of its container.
letter-spacing|The space between characters.
line-height|The height of a line of text.
list-style|Shorthand for the marker type, position and image of a list.
list-style-type|The kind of marker of list items, such as disc or decimal.
margin|The space outside the element's border; one to four values.
max-height|The largest height the element may have.
max-width|The largest width the element may have.
min-height|The smallest height the element may have.
min-width|The smallest width the element may have.
mix-blend-mode|How the element blends with what is behind it.
object-fit|How an image or video is resized to fit its box.
object-position|The alignment of an image or video inside its box.
opacity|The transparency of the element, from 0 (invisible) to 1 (opaque).
order|The order of a flex or grid item, regardless of its position in the markup.
outline|Shorthand for a line drawn outside the border that does not take up space.
outline-offset|The distance between the outline and the border edge.
overflow|What happens to content that does not fit: visible, hidden, scroll or auto.
overflow-wrap word-wrap|Whether long words may break to avoid overflowing.
overflow-x|What happens to content that overflows horizontally.
overflow-y|What happens to content that overflows vertically.
padding|The space between the element's content and its border; one to four values.
perspective|The distance of the viewer for 3D transforms.
place-content|Shorthand for align-content and justify-content.
place-items|Shorthand for align-items and justify-items.
place-self|Shorthand for align-self and justify-self.
pointer-events|Whether the element can be the target of mouse events.
position|How the element is positioned: static, relative, absolute, fixed or sticky.
resize|Whether the user can resize the element, and in which direction.
right|The offset of a positioned element from the right edge of its container.
rotate|Rotates the element, independently of transform.
row-gap|The gap between rows.
scale|Scales the element, independently of transform.
scroll-behavior|Whether scrolling jumps (auto) or animates (smooth).
scroll-snap-type|Makes a scroll container snap to child positions.
tab-size|The width of a tab character.
table-layout|The algorithm used to lay out a table.
text-align|The horizontal alignment of the text.
text-decoration|Underline, overline or strike-through on text; shorthand for line, style and color.
text-indent|The indentation of the first line of text.
text-overflow|How clipped text is signalled, such as with an ellipsis.
text-shadow|Adds shadows to text.
text-transform|Changes the case of text: uppercase, lowercase or capitalize.
top|The offset of a positioned element from the top edge of its container.
touch-action|How a touch screen may handle gestures on the element.
transform|Moves, rotates, scales or skews the element.
transform-origin|The point that transforms are applied around.
transition|Shorthand that animates changes of a property: property, duration, timing function and delay.
transition-delay|How long to wait before a transition starts.
transition-duration|How long a transition takes.
transition-property|Which properties are animated by the transition.
transition-timing-function|The easing curve of the transition.
translate|Moves the element, independently of transform.
user-select|Whether the text of the element can be selected.
vertical-align|The vertical alignment of an inline or table-cell box.
visibility|Hides the element while keeping its space.
white-space|How whitespace and line breaks inside the element are handled.
width|The width of the element.
will-change|Hints to the browser which properties are about to change.
word-break|Where lines may break inside words.
word-spacing|The space between words.
writing-mode|Whether text flows horizontally or vertically.
z-index|The stack order of a positioned element; higher values are drawn on top.
accent-color|The accent color of form controls such as checkboxes.
block-size|The size of the element in the block direction (height in horizontal writing).
inline-size|The size of the element in the inline direction (width in horizontal writing).
color-scheme|Which color schemes, light or dark, the element supports.
contain|Tells the browser the element is independent of the rest of the page, for performance.
container|Shorthand for container-name and container-type.
container-name|The name used to target the container in @container queries.
container-type|Makes the element a query container.
content-visibility|Skips rendering of off-screen content for performance.
margin-block margin-inline|Margin in the block or inline direction, independent of writing mode.
padding-block padding-inline|Padding in the block or inline direction, independent of writing mode.
overscroll-behavior|What happens when the scroll reaches its boundary.
scrollbar-color|The colors of the scrollbar.
scrollbar-gutter|Reserves space for the scrollbar.
scrollbar-width|The thickness of the scrollbar.
text-wrap|How text wraps, such as balance.
view-transition-name|Names the element for view transitions.
zoom|Scales the element's content.
stroke|The color of the outline of an SVG shape.
stroke-width|The thickness of the outline of an SVG shape.
fill-opacity|The opacity of the fill of an SVG shape.
");

        private static readonly Dictionary<string, string> AtRules = Parse(@"
media|Applies the rules inside only when a media query matches, such as a screen width.
supports|Applies the rules inside only when the browser supports a CSS feature.
keyframes|Defines the steps of a CSS animation.
font-face|Defines a custom font to download and use.
import|Imports another stylesheet.
layer|Declares a cascade layer, to control which rules win.
container|Applies the rules inside depending on the size of a container.
charset|Declares the character encoding of the stylesheet.
namespace|Declares an XML namespace for the selectors.
property|Registers a custom property with a type and default value.
page|Sets styles for printed pages.
counter-style|Defines a custom counter style for lists.
scope|Limits the selectors inside to a part of the document.
starting-style|Sets the starting values of a transition when an element first appears.
position-try|Defines a fallback position for anchor positioning.
");

        private static readonly Dictionary<string, string> Pseudos = Parse(@"
hover|Matches while the pointer is over the element.
focus|Matches the element that has keyboard focus.
active|Matches while the element is being pressed.
visited|Matches links that were already visited.
focus-visible|Matches focus that the browser shows, usually from the keyboard.
focus-within|Matches an element that, or whose descendant, has focus.
first-child|Matches an element that is the first child of its parent.
last-child|Matches an element that is the last child of its parent.
nth-child|Matches elements by their position among siblings, such as 2n+1.
only-child|Matches an element that is the only child of its parent.
first-of-type|Matches the first sibling of its element type.
last-of-type|Matches the last sibling of its element type.
nth-of-type|Matches elements by their position among siblings of the same type.
not|Matches elements that do not match the selector inside.
is|Matches elements that match any selector in the list.
where|Like :is, but adds no specificity.
has|Matches an element that contains a match for the selector inside.
checked|Matches checked checkboxes, radio buttons and selected options.
disabled|Matches disabled form controls.
enabled|Matches enabled form controls.
required|Matches form controls that are required.
optional|Matches form controls that are not required.
empty|Matches an element with no children.
root|Matches the root element of the document.
target|Matches the element whose id is in the URL fragment.
placeholder-shown|Matches an input that is showing its placeholder.
read-only|Matches elements that the user cannot edit.
link|Matches links that were not visited yet.
before|Inserts generated content before the element's content.
after|Inserts generated content after the element's content.
first-line|Matches the first line of a block of text.
first-letter|Matches the first letter of a block of text.
placeholder|Matches the placeholder text of an input.
selection|Matches the part of the text the user selected.
marker|Matches the bullet or number of a list item.
backdrop|Matches the backdrop behind a fullscreen element or dialog.
");

        private static readonly Dictionary<string, string> CssFunctions = Parse(@"
var|Inserts the value of a custom property, with an optional fallback.
calc|Computes a value from a mathematical expression, mixing units.
url|Refers to a file by its address.
rgb rgba|Defines a color from red, green and blue values (and alpha).
hsl hsla|Defines a color from hue, saturation and lightness (and alpha).
min|Uses the smallest of the given values.
max|Uses the largest of the given values.
clamp|Keeps a value between a minimum and a maximum around a preferred value.
linear-gradient|An image that blends colors along a straight line.
radial-gradient|An image that blends colors outward from a center point.
conic-gradient|An image that blends colors around a center point.
repeat|Repeats a track list in a grid template.
minmax|A grid track size between a minimum and a maximum.
translate translatex translatey translate3d|Moves the element, used in transform.
rotate rotatex rotatey rotatez|Rotates the element, used in transform.
scale scalex scaley|Scales the element, used in transform.
attr|Uses the value of an HTML attribute of the element.
env|Inserts the value of a browser-defined environment variable.
");

        private static readonly Dictionary<string, string> Graphql = Parse(@"
query|An operation that reads data.
mutation|An operation that changes data on the server.
subscription|An operation that streams updates when data changes.
fragment|A reusable set of fields that can be spread into queries.
on|Names the type a fragment or inline fragment applies to.
type|Defines an object type with its fields.
input|Defines an input object type used for arguments.
enum|Defines a type with a fixed set of values.
interface|Defines a set of fields that other types implement.
union|Defines a type that may be one of several object types.
scalar|Defines a custom scalar type.
schema|Defines the root operation types of the API.
extend|Adds to an existing type.
implements|Declares the interfaces an object type implements.
directive|Defines a directive that can annotate the schema or an operation.
");
    }
}

using System.Globalization;

namespace LabsResponsiveShell.Data;

/// <summary>Relative labels for conversation rows and bubbles.</summary>
public static class RelativeTimeLab
{
    public static DateTime AnchorUtc => DateTime.UtcNow;

    public static string ForUtc(DateTime utc)
    {
        var now = AnchorUtc;
        var local = utc.Kind == DateTimeKind.Utc ? utc : utc.ToUniversalTime();
        var diff = now - local;
        if (diff.TotalMinutes < 1)
        {
            return "now";
        }

        if (diff.TotalMinutes < 60)
        {
            return $"{(int)diff.TotalMinutes}m";
        }

        if (diff.TotalHours < 24 && local.Date == now.Date)
        {
            return $"{(int)diff.TotalHours}h";
        }

        if (local.Date == now.Date.AddDays(-1))
        {
            return "Yesterday";
        }

        var days = (int)(now.Date - local.Date).TotalDays;
        return days < 7
            ? local.ToString("ddd", CultureInfo.InvariantCulture)
            : local.ToString(
                local.Year == now.Year ? "MMM d" : "MMM d, yyyy",
                CultureInfo.InvariantCulture);
    }
}

public enum PresenceGlyph
{
    None,
    Online,
    Away,
}

public sealed class MockConversation
{
    public required string Id { get; init; }

    /// <summary>Display title: person name or group title.</summary>
    public required string Title { get; init; }

    public bool IsGroup { get; init; }

    public int? GroupMemberCount { get; init; }

    /// <summary>Single-line preview unless <see cref="IsTyping"/> is true.</summary>
    public required string Preview { get; init; }

    /// <summary>When true, row shows typing affordance instead of preview.</summary>
    public bool IsTyping { get; init; }

    public required string RelativeTimeLabel { get; init; }

    public bool IsPinned { get; init; }

    public int UnreadCount { get; init; }

    public PresenceGlyph Presence { get; init; }

    public required IReadOnlyList<MockThreadItem> Messages { get; init; }

    /// <summary>Subtitle under name in thread header for DMs.</summary>
    public string ThreadPresenceSubtitle { get; init; } = string.Empty;

    /// <summary>Typing line at bottom e.g. "Sara is typing". Empty when none.</summary>
    public string TypingLine { get; init; } = string.Empty;

    /// <summary>First name or short handle for typing label.</summary>
    public required string PeerFirstName { get; init; }
}

public enum MockThreadKind
{
    DateSeparator,
    Text,
    ImagePlaceholder,
    VoiceStub,
    LinkPreview,
}

public sealed class MockThreadItem
{
    public MockThreadKind Kind { get; init; }

    public bool IsMine { get; init; }

    public string Body { get; init; } = string.Empty;

    public string RelativeTimeLabel { get; init; } = string.Empty;

    public bool ShowAvatar { get; init; } = true;

    public bool TightVerticalSpacing { get; init; }

    public Thickness ChatRowPadding => new(0, TightVerticalSpacing ? 2 : 8, 0, 8);

    public string? ReplyQuoteAuthor { get; init; }

    public string? ReplyQuoteSnippet { get; init; }

    public string? ReactionGlyph { get; init; }

    public int ReactionCount { get; init; }

    public string? ImageCaption { get; init; }

    public string? LinkTitle { get; init; }

    public string? LinkDescription { get; init; }

    public string? LinkDomain { get; init; }

    /// <summary>Voice duration label.</summary>
    public string VoiceDuration { get; init; } = "0:23";
}

public static class MockConversations
{
    private static IReadOnlyList<MockConversation>? _all;

    public static IReadOnlyList<MockConversation> GetAll()
    {
        _all ??= BuildAll();
        return _all;
    }

    /// <summary>Home dashboard: recent four.</summary>
    public static IReadOnlyList<MockConversation> GetHomeRecent(int count = 4) =>
        [.. GetAll().Where(c => !c.IsTyping).Take(count)];

    public static MockConversation? Find(string id)
    {
        foreach (var c in GetAll())
        {
            if (c.Id == id)
            {
                return c;
            }
        }

        return null;
    }

    private static IReadOnlyList<MockConversation> BuildAll()
    {
        var now = RelativeTimeLab.AnchorUtc;
        var t1 = now.AddMinutes(-18);
        var tYesterday = now.AddDays(-1).AddHours(14);
        var tWeek = now.AddDays(-3).AddHours(9);

        MockThreadItem T(string body, bool mine, DateTime utc, bool showAvatar = true, bool tight = false)
        {
            return new MockThreadItem
            {
                Kind = MockThreadKind.Text,
                IsMine = mine,
                Body = body,
                RelativeTimeLabel = RelativeTimeLab.ForUtc(utc),
                ShowAvatar = showAvatar,
                TightVerticalSpacing = tight,
            };
        }

        var saraMsgs = BuildSaraThread(now);

        IReadOnlyList<MockThreadItem> BuildSaraThread(DateTime nowAnchor)
        {
            var msgs = new List<MockThreadItem>
            {
                new()
                {
                    Kind = MockThreadKind.DateSeparator,
                    Body = "Today",
                },
                T("Can you send the ADR when you get a sec?", false, nowAnchor.AddMinutes(-55)),
                T("Yep, pushed the fix to main", true, nowAnchor.AddMinutes(-52)),
                new()
                {
                    Kind = MockThreadKind.LinkPreview,
                    IsMine = false,
                    RelativeTimeLabel = RelativeTimeLab.ForUtc(nowAnchor.AddMinutes(-49)),
                    ShowAvatar = true,
                    Body = "",
                    LinkTitle = "ADR 014: Responsive shell tokens",
                    LinkDescription = "How we migrate layout chrome to Aurora tokens across WASM and desktop.",
                    LinkDomain = "github.com",
                },
                T("I updated the Figma", false, nowAnchor.AddMinutes(-45), showAvatar: false, tight: true),
                T("See you tomorrow at 2", true, nowAnchor.AddMinutes(-40)),
                new()
                {
                    Kind = MockThreadKind.Text,
                    IsMine = false,
                    Body = "Let me pull the numbers for the deck.",
                    RelativeTimeLabel = RelativeTimeLab.ForUtc(nowAnchor.AddMinutes(-35)),
                    ShowAvatar = true,
                    ReplyQuoteAuthor = "You",
                    ReplyQuoteSnippet = "See you tomorrow at 2",
                },
                new()
                {
                    Kind = MockThreadKind.ImagePlaceholder,
                    IsMine = true,
                    RelativeTimeLabel = RelativeTimeLab.ForUtc(nowAnchor.AddMinutes(-28)),
                    ShowAvatar = true,
                    ImageCaption = "Whiteboard from the review",
                },
                new()
                {
                    Kind = MockThreadKind.VoiceStub,
                    IsMine = false,
                    RelativeTimeLabel = RelativeTimeLab.ForUtc(nowAnchor.AddMinutes(-15)),
                    ShowAvatar = true,
                    VoiceDuration = "0:23",
                },
                T("Sounds good, will review tonight", true, nowAnchor.AddMinutes(-12), showAvatar: false, tight: true),
                new()
                {
                    Kind = MockThreadKind.Text,
                    IsMine = false,
                    Body = "Thanks, that unblocks the release note.",
                    RelativeTimeLabel = RelativeTimeLab.ForUtc(nowAnchor.AddMinutes(-8)),
                    ShowAvatar = true,
                    ReactionGlyph = "\u2665",
                    ReactionCount = 2,
                },
            };
            return msgs;
        }

        return
        [
            new MockConversation
            {
                Id = "sara",
                Title = "Sara Ahmed",
                Preview = "Thanks, that unblocks the release note.",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddMinutes(-8)),
                IsPinned = true,
                UnreadCount = 2,
                Presence = PresenceGlyph.Online,
                PeerFirstName = "Sara",
                ThreadPresenceSubtitle = "online",
                TypingLine = "Sara is typing",
                Messages = saraMsgs,
            },
            new MockConversation
            {
                Id = "alex",
                Title = "Alex Rivers",
                Preview = "Let me pull the numbers",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(t1),
                UnreadCount = 5,
                Presence = PresenceGlyph.None,
                PeerFirstName = "Alex",
                ThreadPresenceSubtitle = "last seen 2h ago",
                Messages =
                [
                    T("Did the export land in prod?", false, t1.AddHours(-2)),
                    T("It is live as of this morning", true, t1.AddHours(-1).AddMinutes(-40)),
                    T("Let me pull the numbers", false, t1),
                ],
            },
            new MockConversation
            {
                Id = "maya",
                Title = "Maya Patel",
                Preview = "",
                IsTyping = true,
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddMinutes(-5)),
                Presence = PresenceGlyph.Away,
                PeerFirstName = "Maya",
                ThreadPresenceSubtitle = "away",
                TypingLine = "Maya is typing",
                Messages =
                [
                    T("I will sync with design before noon", false, now.AddMinutes(-40)),
                ],
            },
            new MockConversation
            {
                Id = "design",
                Title = "Design review",
                IsGroup = true,
                GroupMemberCount = 3,
                Preview = "Aurora tokens are in for the rail",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddMinutes(-22)),
                Presence = PresenceGlyph.None,
                PeerFirstName = "Design",
                ThreadPresenceSubtitle = "3 members",
                Messages =
                [
                    T("The accent felt hot at full strength", false, now.AddMinutes(-80)),
                    T("Pulled it back to 70 on dark", true, now.AddMinutes(-75)),
                    T("Aurora tokens are in for the rail", false, now.AddMinutes(-22)),
                ],
            },
            new MockConversation
            {
                Id = "james",
                Title = "James Ogilvie",
                Preview = "See you tomorrow at 2",
                RelativeTimeLabel = "Yesterday",
                Presence = PresenceGlyph.Away,
                PeerFirstName = "James",
                ThreadPresenceSubtitle = "away",
                Messages =
                [
                    T("Can you own the rollout checklist?", false, tYesterday),
                    T("On it", true, tYesterday.AddMinutes(4)),
                    T("See you tomorrow at 2", false, tYesterday.AddMinutes(20)),
                ],
            },
            new MockConversation
            {
                Id = "priya",
                Title = "Priya Rao",
                Preview = "Pushed the fix to main",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddMinutes(-90)),
                UnreadCount = 13,
                Presence = PresenceGlyph.Online,
                PeerFirstName = "Priya",
                ThreadPresenceSubtitle = "online",
                Messages =
                [
                    T("CI is red on the wasm head", false, now.AddMinutes(-120)),
                    T("Pushed the fix to main", true, now.AddMinutes(-90)),
                ],
            },
            new MockConversation
            {
                Id = "iosbeta",
                Title = "iOS beta squad",
                IsGroup = true,
                GroupMemberCount = 5,
                Preview = "Build 412 is on TestFlight",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddHours(-4)),
                PeerFirstName = "iOS",
                ThreadPresenceSubtitle = "5 members",
                Messages =
                [
                    T("Crash on first open for 2 users", false, now.AddHours(-6)),
                    T("Repro steps in the thread", true, now.AddHours(-5).AddMinutes(10)),
                    T("Build 412 is on TestFlight", false, now.AddHours(-4)),
                ],
            },
            new MockConversation
            {
                Id = "ben",
                Title = "Ben Khoury",
                Preview = "I updated the Figma",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(tWeek),
                Presence = PresenceGlyph.None,
                PeerFirstName = "Ben",
                ThreadPresenceSubtitle = "last seen 1d ago",
                Messages =
                [
                    T("Spacing on the chip looks tight on 320", false, tWeek.AddHours(-1)),
                    T("I updated the Figma", true, tWeek),
                ],
            },
            new MockConversation
            {
                Id = "elena",
                Title = "Elena Torres",
                Preview = "Yep, on my list for Friday",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddDays(-2).AddHours(16)),
                Presence = PresenceGlyph.Online,
                PeerFirstName = "Elena",
                ThreadPresenceSubtitle = "online",
                Messages =
                [
                    T("Can you review the copy deck?", false, now.AddDays(-2).AddHours(15)),
                    T("Yep, on my list for Friday", true, now.AddDays(-2).AddHours(16)),
                ],
            },
            new MockConversation
            {
                Id = "tom",
                Title = "Tom Yoshida",
                Preview = "Routing looks good in staging",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddDays(-4).AddHours(11)),
                Presence = PresenceGlyph.None,
                PeerFirstName = "Tom",
                ThreadPresenceSubtitle = "last seen 4d ago",
                Messages =
                [
                    T("Did you get eyes on the edge cache headers?", false, now.AddDays(-4).AddHours(10)),
                    T("Routing looks good in staging", true, now.AddDays(-4).AddHours(11)),
                ],
            },
            new MockConversation
            {
                Id = "weekly",
                Title = "Weekly sync",
                IsGroup = true,
                GroupMemberCount = 6,
                Preview = "Notes are in the doc",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddDays(-5)),
                PeerFirstName = "Weekly",
                ThreadPresenceSubtitle = "6 members",
                Messages =
                [
                    T("Agenda is short today", false, now.AddDays(-5).AddMinutes(-30)),
                    T("Notes are in the doc", true, now.AddDays(-5)),
                ],
            },
            new MockConversation
            {
                Id = "chris",
                Title = "Chris Lee",
                Preview = "Thanks for the quick turnaround",
                RelativeTimeLabel = RelativeTimeLab.ForUtc(now.AddDays(-6)),
                UnreadCount = 1,
                Presence = PresenceGlyph.Away,
                PeerFirstName = "Chris",
                ThreadPresenceSubtitle = "away",
                Messages =
                [
                    T("Need the signed PDF by EOD if possible", false, now.AddDays(-6).AddHours(-2)),
                    T("Thanks for the quick turnaround", true, now.AddDays(-6)),
                ],
            },
        ];
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpSolid.Oop.Encapsulation;

public class DocumentService
{
    // constants
    private const int MAX_CONTENT_SIZE = 50 * 1024 * 1024;
    private const int MAX_TITLE_LENGTH = 200;

    private string _title;
    private string _content;
    private string _filename;
    private DateTime _createdDate;
    private DateTime _modifiedDate;
    private string _createdBy;
    private string _modifiedBy;
    private DocumentStatus _status;
    private List<DocumentVersion> _versions;
    private List<string> _tags;
    private AccessLevel _accessLevel;
    private byte[] _fileContent;
    private readonly string _documentId;

    // Constructor
    public DocumentService(string title, byte[] content, string filename, string createdBy, AccessLevel accessLevel)
    {
        _documentId = Guid.NewGuid().ToString();

        Title = title;
        FileName = filename;
        CreatedBy = createdBy;
        AccessLevel = accessLevel;

        _modifiedDate = _createdDate;
        _createdDate = DateTime.UtcNow;
        _status = DocumentStatus.Draft;
        _versions = new List<DocumentVersion>();
        _tags = new List<string>();

        SetContent(content, createdBy);
        CreateInitialVersion(createdBy);
    }

    // Public methods with validations
    public string DocumentId => _documentId;
    public DateTime CreatedDate => _createdDate;
    public DateTime ModifiedDate => _modifiedDate;
    public DocumentStatus Status => _status;
    public long SizeInBytes => _fileContent.Length;
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();
    public IReadOnlyList<DocumentVersion> Versions => _versions.AsReadOnly();

    public string CreatedBy
    {
        get => _createdBy;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("User creator cannot be null or empty.");

            _createdBy = value;
        }
    }
    public AccessLevel AccessLevel
    {
        get => _accessLevel;
        set
        {
            if (!Enum.IsDefined(typeof(AccessLevel), value))
                throw new ArgumentException("Invalid access level.");
            _accessLevel = value;
        }
    }

    public string ModifiedBy => _modifiedBy;

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be null or empty.");

            if (value.Length > MAX_TITLE_LENGTH)
                throw new ArgumentException($"Title length exceeds the limit of {MAX_TITLE_LENGTH} characters.");

            _title = value;
        }
    }

    public string FileName
    {
        get => _filename;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("File name cannot be null or empty.");

            if (!IsValidFileName(value))
                throw new ArgumentException("Invalid file name.");

            _filename = value;
        }
    }

    // Controlled operations
    public void UpdateContent(byte[] content, string modifiedBy)
    {
        ValidateEditAccess(modifiedBy);
        SetContent(content, modifiedBy);
        CreateVersion($"Document updated by {modifiedBy}", modifiedBy);
    }

    public void Publish(string userId)
    {
        ValidateEditAccess(userId);

        if (_status == DocumentStatus.Published)
            throw new InvalidOperationException("Document is already published.");

        _status = DocumentStatus.Published;
        _modifiedBy = userId;
        _modifiedDate = DateTime.UtcNow;

        CreateVersion($"Document published by {userId}", userId);
    }

    public void Archive(string userId)
    {
        if (_status == DocumentStatus.Archived)
            throw new InvalidOperationException("Document is already archived.");

        ValidateEditAccess(userId);

        _status = DocumentStatus.Archived;
        _modifiedBy = userId;
        _modifiedDate = DateTime.UtcNow;

        CreateVersion($"Document archived by {userId}", userId);
    }

    public byte[] GetContent(string documentId, string userId)
    {
        ValidateReadAccess(userId);

        if (documentId != _documentId)
            throw new ArgumentException("Invalid document ID. Access denied.");

        if (_fileContent == null || _fileContent.Length == 0)
            throw new InvalidOperationException("Document content is empty.");

        byte[] contentCopy = new byte[_fileContent.Length];
        Array.Copy(_fileContent, contentCopy, _fileContent.Length);

        return contentCopy;
    }

    public void AddTag(string tag, string userId)
    {
        ValidateEditAccess(userId);

        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be null or empty.");

        var normalizedTag = tag.Trim().ToLower();

        if (_tags.Contains(normalizedTag))
            throw new InvalidOperationException($"Tag '{normalizedTag}' already exists.");

        _tags.Add(normalizedTag);
        _modifiedBy = userId;
        _modifiedDate = DateTime.UtcNow;
    }

    public void RemoveTag(string tag, string userId)
    {
        ValidateEditAccess(userId);

        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be null or empty.");

        var normalizedTag = tag.Trim().ToLower();

        if (!_tags.Contains(normalizedTag))
            throw new InvalidOperationException($"Tag '{normalizedTag}' does not exist.");

        _tags.Remove(normalizedTag);
        _modifiedBy = userId;
        _modifiedDate = DateTime.UtcNow;
    }

    // Privates methods
    private void CreateVersion(string changes, string createdBy)
    {
        _versions.Add(new DocumentVersion
        {
            VersionNumber = _versions.Count + 1,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            Changes = changes,
            SizeInBytes = _fileContent.Length
        });
    }

    private void ValidateEditAccess(string userId)
    {
        ValidateReadAccess(userId);

        if (_accessLevel == AccessLevel.ReadOnly)
            throw new UnauthorizedAccessException("You do not have permission to edit this document.");

        if (_status == DocumentStatus.Archived)
            throw new InvalidOperationException("You cannot edit an archived document.");
    }

    private void ValidateReadAccess(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User ID authentication required.");

        if (_accessLevel == AccessLevel.Restricted && userId != _createdBy)
            throw new UnauthorizedAccessException("You do not have permission to access this document.");
    }

    private bool IsValidFileName(string fileName)
    {
        // Check for null or empty
        if (string.IsNullOrWhiteSpace(fileName))
            return false;
        // Check for invalid characters
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return false;

        return true;
    }

    private byte[] SetContent(byte[] content, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy cannot be null or empty.");

        if (content == null || content.Length == 0)
            throw new ArgumentException("Content cannot be null or empty.");

        if (content.Length > MAX_CONTENT_SIZE) // 50MB limit
            throw new ArgumentException($"Content size exceeds the limit of {MAX_CONTENT_SIZE / (1024 * 1024)}MB.");

        _fileContent = new byte[content.Length];
        Array.Copy(content, _fileContent, content.Length);

        _modifiedBy = modifiedBy;
        _modifiedDate = DateTime.UtcNow;

        return _fileContent;
    }

    private void CreateInitialVersion(string createdBy)
    {
        _versions.Add(new DocumentVersion
        {
            VersionNumber = 1,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            Changes = "Initial version",
            SizeInBytes = _fileContent.Length
        });
    }
}

public class DocumentVersion
{
    public int VersionNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string Changes { get; set; }
    public long SizeInBytes { get; set; }
}

public enum DocumentStatus
{
    Draft,
    UnderReview,
    Published,
    Archived
}

public enum AccessLevel
{
    Public,
    Internal,
    Restricted,
    ReadOnly
}

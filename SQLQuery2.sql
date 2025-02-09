SELECT 
    i.name AS IndexName,
    t.name AS TableName,
    c.name AS ColumnName
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.index_id = ic.index_id AND i.object_id = ic.object_id
INNER JOIN 
    sys.columns c ON ic.column_id = c.column_id AND ic.object_id = c.object_id
INNER JOIN 
    sys.tables t ON i.object_id = t.object_id
WHERE 
    c.name = 'ProviderKey' AND t.name = 'AspNetUserLogins';

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[files_evidencias_only_upload](
	[id_files_evidencias] [int] IDENTITY(1,1) NOT NULL,
	[file_name] [nvarchar](100) NOT NULL,
	[file_type] [nvarchar](10) NOT NULL,
	[file_content] [varbinary](max) NOT NULL,
	[id_derivacion] [int] NOT NULL,
 CONSTRAINT [PK_files_evidencias_only_upload] PRIMARY KEY CLUSTERED 
(
	[id_files_evidencias] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[files_evidencias_only_upload] ADD  CONSTRAINT [DEFAULT_files_evidencias_only_upload_file_name]  DEFAULT ('evidencia por defecto') FOR [file_name]
GO
ALTER TABLE [dbo].[files_evidencias_only_upload] ADD  CONSTRAINT [DEFAULT_files_evidencias_only_upload_file_type]  DEFAULT ('.txt') FOR [file_type]
GO

INSERT INTO [dbo].[files_evidencias_only_upload] ([file_name], [file_type], [file_content], [id_derivacion])
VALUES ('evidencia por defecto', '.txt', 0x444451ABC45, 0);
GO

SELECT * FROM [dbo].[files_evidencias_only_upload];
DROP TABLE [dbo].[files_evidencias_only_upload];


SELECT TOP 100 * FROM derivaciones_asesores ORDER BY id_derivacion DESC;

ALTER TABLE derivaciones_asesores
ADD fue_procesada_evidencia BIT NULL;

ALTER TABLE derivaciones_asesores
ADD CONSTRAINT DF_derivaciones_asesores_fue_procesada_evidencia DEFAULT 0 FOR fue_procesada_evidencia;


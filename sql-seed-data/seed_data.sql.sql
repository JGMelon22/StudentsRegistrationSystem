-- Insert para Alunos
INSERT INTO StudentsRegistrationSystem.dbo.Alunos(Id, Nome, Email, DataNascimento, CreatedAt, UpdatedAt)
VALUES
('A1B2C3D4-E5F6-7890-ABCD-EF1234567890', 'João Silva', 'joao.silva@email.com', '1995-03-15', GETDATE(), GETDATE()),
('B2C3D4E5-F6A7-8901-BCDE-F12345678901', 'Maria Santos', 'maria.santos@email.com', '1998-07-22', GETDATE(), GETDATE()),
('C3D4E5F6-A7B8-9012-CDEF-123456789012', 'Pedro Oliveira', 'pedro.oliveira@email.com', '1997-11-08', GETDATE(), GETDATE()),
('D4E5F6A7-B8C9-0123-DEF1-234567890123', 'Ana Costa', 'ana.costa@email.com', '1999-05-30', GETDATE(), GETDATE()),
('E5F6A7B8-C9D0-1234-EF12-345678901234', 'Carlos Ferreira', 'carlos.ferreira@email.com', '1996-12-12', GETDATE(), GETDATE());

-- Insert para Cursos
INSERT INTO StudentsRegistrationSystem.dbo.Cursos(Id, Nome, Descricao, CreatedAt, UpdatedAt)
VALUES
('F6A7B8C9-D0E1-2345-F123-456789012345', 'Desenvolvimento Web Full Stack', 'Curso completo de desenvolvimento web com HTML, CSS, JavaScript, Node.js e React', GETDATE(), GETDATE()),
('A7B8C9D0-E1F2-3456-1234-567890123456', 'Ciência de Dados com Python', 'Aprenda análise de dados, machine learning e visualização com Python', GETDATE(), GETDATE()),
('B8C9D0E1-F2A3-4567-2345-678901234567', 'DevOps e Cloud Computing', 'Domine práticas DevOps, Docker, Kubernetes e AWS', GETDATE(), GETDATE()),
('C9D0E1F2-A3B4-5678-3456-789012345678', 'Mobile Development com React Native', 'Desenvolva aplicativos mobile para iOS e Android', GETDATE(), GETDATE());

-- Insert para Matrículas
INSERT INTO StudentsRegistrationSystem.dbo.Matriculas(Id, AlunoId, CursoId, DataMatricula, Ativa, CreatedAt, UpdatedAt)
VALUES
(NEWID(), 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890', 'F6A7B8C9-D0E1-2345-F123-456789012345', '2024-02-01', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'B2C3D4E5-F6A7-8901-BCDE-F12345678901', 'A7B8C9D0-E1F2-3456-1234-567890123456', '2024-02-03', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'C3D4E5F6-A7B8-9012-CDEF-123456789012', 'B8C9D0E1-F2A3-4567-2345-678901234567', '2024-02-05', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'D4E5F6A7-B8C9-0123-DEF1-234567890123', 'F6A7B8C9-D0E1-2345-F123-456789012345', '2024-02-07', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'E5F6A7B8-C9D0-1234-EF12-345678901234', 'C9D0E1F2-A3B4-5678-3456-789012345678', '2024-02-10', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890', 'A7B8C9D0-E1F2-3456-1234-567890123456', '2024-02-12', CONVERT(BIT, 1), GETDATE(), GETDATE()),
(NEWID(), 'B2C3D4E5-F6A7-8901-BCDE-F12345678901', 'C9D0E1F2-A3B4-5678-3456-789012345678', '2024-02-15', CONVERT(BIT, 0), GETDATE(), GETDATE());
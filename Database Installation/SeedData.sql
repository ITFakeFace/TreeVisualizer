-- NOTE: Not Including Tree Data
-- Use the database (assuming it's already created)
-- USE MultipleChoiceApplication;

-- Seed data for Users table
INSERT INTO `Users` (`email`, `username`, `password`) VALUES
('alice.smith@example.com', 'alice_s', 'password123'),
('bob.johnson@example.com', 'bob_j', 'securepass'),
('charlie.brown@example.com', 'charlie_b', 'mysecretpwd'),
('diana.prince@example.com', 'diana_p', 'wonderwoman'),
('eve.adams@example.com', 'eve_a', 'applepie');

-- Seed data for Quizzes table
-- Quiz 1: Binary Tree Fundamentals (created by Alice)
INSERT INTO `Quizzes` (`title`, `type`, `is_random`, `attemp_number`, `created_by`, `is_result_showable`, `start_at`, `end_at`, `time_limit`) VALUES
('Binary Tree Fundamentals', 'Multiple Choice', TRUE, 3, 1, TRUE, '2024-01-10 09:00:00', '2024-01-10 10:00:00', '00:15:00');

-- Quiz 2: Binary Search Tree Concepts (created by Bob)
INSERT INTO `Quizzes` (`title`, `type`, `is_random`, `attemp_number`, `created_by`, `is_result_showable`, `start_at`, `end_at`, `time_limit`) VALUES
('Binary Search Tree Concepts', 'Multiple Choice', FALSE, 1, 2, TRUE, '2024-02-15 14:00:00', '2024-02-15 15:00:00', '00:20:00');

-- Quiz 3: AVL Tree Balancing (created by Alice)
INSERT INTO `Quizzes` (`title`, `type`, `is_random`, `attemp_number`, `created_by`, `is_result_showable`, `start_at`, `end_at`, `time_limit`) VALUES
('AVL Tree Balancing', 'Multiple Choice', TRUE, 2, 1, FALSE, '2024-03-20 11:00:00', '2024-03-20 12:00:00', '00:25:00');

-- Quiz 4: Advanced Tree Structures (created by Charlie)
INSERT INTO `Quizzes` (`title`, `type`, `is_random`, `attemp_number`, `created_by`, `is_result_showable`, `start_at`, `end_at`, `time_limit`) VALUES
('Advanced Tree Structures', 'Multiple Choice', TRUE, 5, 3, TRUE, '2024-04-01 10:00:00', '2024-04-01 11:30:00', '00:30:00');


-- Seed data for QuizzDetails table (Questions for each quiz)

-- Questions for Quiz 1: Binary Tree Fundamentals (quizz_id = 1)
INSERT INTO `QuizzDetails` (`quizz_id`, `question`, `answer1`, `answer2`, `answer3`, `answer4`, `correct_answer`) VALUES
(1, 'What is the maximum number of nodes at level \'l\' (root at level 0) in a binary tree?', 'l', '$2l$', '$2^l$', '$l^2$', 3),
(1, 'What is the height of a binary tree with \'n\' nodes in the worst case?', '$log_2 n$', '$n$', '$n-1$', '$n/2$', 3),
(1, 'In a full binary tree, if there are \'L\' leaves, what is the total number of nodes?', '$2L-1$', '$L+1$', '$2L$', '$L-1$', 1),
(1, 'Which of the following is NOT a type of binary tree traversal?', 'In-order', 'Pre-order', 'Post-order', 'Depth-order', 4);

-- Questions for Quiz 2: Binary Search Tree Concepts (quizz_id = 2)
INSERT INTO `QuizzDetails` (`quizz_id`, `question`, `answer1`, `answer2`, `answer3`, `answer4`, `correct_answer`) VALUES
(2, 'In a Binary Search Tree (BST), where is the smallest element typically found?', 'Root node', 'Rightmost node', 'Leftmost node', 'Any leaf node', 3),
(2, 'What is the average time complexity for searching an element in a balanced BST?', '$O(n)$', '$O(log n)$', '$O(n log n)$', '$O(1)$', 2),
(2, 'Which traversal of a BST visits nodes in non-decreasing order?', 'Pre-order traversal', 'Post-order traversal', 'In-order traversal', 'Level-order traversal', 3),
(2, 'If you insert a new node into a BST, where is it always inserted?', 'At the root', 'At a leaf node', 'In the middle of the tree', 'Randomly', 2);

-- Questions for Quiz 3: AVL Tree Balancing (quizz_id = 3)
INSERT INTO `QuizzDetails` (`quizz_id`, `question`, `answer1`, `answer2`, `answer3`, `answer4`, `correct_answer`) VALUES
(3, 'What is the maximum allowed balance factor for any node in an AVL tree?', '0', '1', '2', 'Any integer', 2),
(3, 'Which rotation is performed when a new node is inserted into the right subtree of the left child (RL case) in an AVL tree?', 'Left-Left rotation', 'Right-Right rotation', 'Left-Right rotation', 'Right-Left rotation', 4),
(3, 'What is the primary purpose of an AVL tree?', 'To store data randomly', 'To maintain a balanced tree for efficient search operations', 'To allow duplicate keys', 'To simplify deletion operations', 2),
(3, 'An AVL tree is a self-balancing binary search tree. What does "self-balancing" imply?', 'It automatically sorts elements', 'It adjusts its structure to maintain balance after insertions/deletions', 'It can balance itself without any operations', 'It only allows balanced insertions', 2);

-- Questions for Quiz 4: Advanced Tree Structures (quizz_id = 4)
INSERT INTO `QuizzDetails` (`quizz_id`, `question`, `answer1`, `answer2`, `answer3`, `answer4`, `correct_answer`) VALUES
(4, 'What is a B-tree primarily used for?', 'In-memory data storage', 'Disk-based data storage', 'Graph traversal', 'Network routing', 2),
(4, 'In a Red-Black Tree, what color is the root node always?', 'Red', 'Black', 'Green', 'Blue', 2),
(4, 'Which tree data structure is often used to implement symbol tables in compilers?', 'Binary Tree', 'AVL Tree', 'Trie', 'Heap', 3),
(4, 'What is the main advantage of a B+ tree over a B-tree?', 'Faster insertion', 'Faster deletion', 'Efficient range queries', 'Less memory usage', 3);


-- Seed data for Attemps table

-- Attempt 1: Bob (id=2) attempts Quiz 1 (id=1)
INSERT INTO `Attemps` (`answered_by`, `quizz_id`, `correct_number`, `time`, `complete`) VALUES
(2, 1, 3, '00:10:30', TRUE);

-- Attempt 2: Charlie (id=3) attempts Quiz 1 (id=1)
INSERT INTO `Attemps` (`answered_by`, `quizz_id`, `correct_number`, `time`, `complete`) VALUES
(3, 1, 4, '00:08:15', TRUE);

-- Attempt 3: Alice (id=1) attempts Quiz 2 (id=2)
INSERT INTO `Attemps` (`answered_by`, `quizz_id`, `correct_number`, `time`, `complete`) VALUES
(1, 2, 3, '00:12:00', TRUE);

-- Attempt 4: Diana (id=4) attempts Quiz 3 (id=3)
INSERT INTO `Attemps` (`answered_by`, `quizz_id`, `correct_number`, `time`, `complete`) VALUES
(4, 3, 2, '00:18:45', TRUE);

-- Attempt 5: Eve (id=5) attempts Quiz 4 (id=4) - Incomplete
INSERT INTO `Attemps` (`answered_by`, `quizz_id`, `correct_number`, `time`, `complete`) VALUES
(5, 4, 1, '00:05:00', FALSE);


-- Seed data for Answers table

-- Answers for Attempt 1 (attemp_id = 1, Bob on Quiz 1)
-- Quiz 1 Questions:
-- Q1.1: Max nodes at level 'l' -> $2^l$ (Correct: 3)
-- Q1.2: Height worst case -> n-1 (Correct: 3)
-- Q1.3: Full binary tree nodes -> $2L-1$ (Correct: 1)
-- Q1.4: NOT a traversal -> Depth-order (Correct: 4)
INSERT INTO `Answers` (`question_id`, `attemp_id`, `answer`) VALUES
(1, 1, 3), -- Correct for Q1.1 (id=1)
(2, 1, 3), -- Correct for Q1.2 (id=2)
(3, 1, 1), -- Correct for Q1.3 (id=3)
(4, 1, 4); -- Correct for Q1.4 (id=4)

-- Answers for Attempt 2 (attemp_id = 2, Charlie on Quiz 1)
INSERT INTO `Answers` (`question_id`, `attemp_id`, `answer`) VALUES
(1, 2, 3), -- Correct for Q1.1
(2, 2, 3), -- Correct for Q1.2
(3, 2, 1), -- Correct for Q1.3
(4, 2, 4); -- Correct for Q1.4

-- Answers for Attempt 3 (attemp_id = 3, Alice on Quiz 2)
-- Quiz 2 Questions:
-- Q2.1: Smallest element in BST -> Leftmost (Correct: 3)
-- Q2.2: Search time balanced BST -> O(log n) (Correct: 2)
-- Q2.3: Non-decreasing order BST -> In-order (Correct: 3)
-- Q2.4: New node insertion -> At a leaf (Correct: 2)
INSERT INTO `Answers` (`question_id`, `attemp_id`, `answer`) VALUES
(5, 3, 3), -- Correct for Q2.1 (id=5)
(6, 3, 1), -- Incorrect for Q2.2 (id=6) - Alice answered 1 instead of 2
(7, 3, 3), -- Correct for Q2.3 (id=7)
(8, 3, 2); -- Correct for Q2.4 (id=8)

-- Answers for Attempt 4 (attemp_id = 4, Diana on Quiz 3)
-- Quiz 3 Questions:
-- Q3.1: Max balance factor AVL -> 1 (Correct: 2)
-- Q3.2: RL case rotation -> Right-Left (Correct: 4)
-- Q3.3: Primary purpose AVL -> Balanced tree for search (Correct: 2)
-- Q3.4: Self-balancing implies -> Adjusts structure (Correct: 2)
INSERT INTO `Answers` (`question_id`, `attemp_id`, `answer`) VALUES
(9, 4, 2),  -- Correct for Q3.1 (id=9)
(10, 4, 4), -- Correct for Q3.2 (id=10)
(11, 4, 1), -- Incorrect for Q3.3 (id=11) - Diana answered 1 instead of 2
(12, 4, 2); -- Correct for Q3.4 (id=12)

-- Answers for Attempt 5 (attemp_id = 5, Eve on Quiz 4) - Incomplete, only answered 1 question
-- Quiz 4 Questions:
-- Q4.1: B-tree used for -> Disk-based (Correct: 2)
INSERT INTO `Answers` (`question_id`, `attemp_id`, `answer`) VALUES
(13, 5, 2); -- Correct for Q4.1 (id=13)
